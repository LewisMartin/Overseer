using Newtonsoft.Json;
using Overseer.DTOs.MonitoringConfig;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Overseer.WebApp.WebApi
{
    public class MonitoringConfigEndpointController : BaseApiController
    {
        // POST: endpoint for authentication & passcode generation for monitoring agent config wizard
        [HttpPost]
        public MonitoringAgentConfigResponse AuthenticateMonitoringAgentConfiguration([FromBody] string monitoringConfigDetails)
        {
            MonitoringAgentConfigRequest monAgentConfigReq = JsonConvert.DeserializeObject<MonitoringAgentConfigRequest>(monitoringConfigDetails);

            Machine machine = _unitOfWork.Machines.GetMachineAndOwner(monAgentConfigReq.MachineGuid);

            // validate user's credentials & ensure user is creator of this machine's environment
            if ((!_ValidatePassword(monAgentConfigReq.Username, monAgentConfigReq.Password)) || (machine.TestEnvironment.UserAccount.UserName != monAgentConfigReq.Username))
            {
                return new MonitoringAgentConfigResponse()
                {
                    Success = false,
                    MachineSecret = null
                };
            }

            // generate a new machine secret
            string newSecret = _NewMachineSecret();

            MonitoringAgentCredential monAgentCred = _unitOfWork.MonitoringAgentCredentials.Get(monAgentConfigReq.MachineGuid);

            if (monAgentCred != null)
            {
                // update existing entry
                monAgentCred.MonitoringAgentSecret = newSecret;
            }
            else
            {
                // create new entry
                monAgentCred = new MonitoringAgentCredential()
                {
                    MachineID = machine.MachineID,
                    MonitoringAgentSecret = newSecret
                };

                _unitOfWork.MonitoringAgentCredentials.Add(monAgentCred);
            }
            _unitOfWork.Save();

            // return secret in response
            return new MonitoringAgentConfigResponse()
            {
                Success = true,
                MachineSecret = newSecret
            };
        }

        // this REALLY needs to be moved into a service layer.. LIKE NOW@@@@
        private bool _ValidatePassword(string userName, string userSecret)
        {
            // bool to store whether password is validated/not
            bool valid = false;

            // create and instance of our hashing provider
            var crypto = new SimpleCrypto.PBKDF2();

            // getting user via user repository
            var matchedUser = _unitOfWork.Users.GetUserByUsername(userName);

            // if a user is matched in the db
            if (matchedUser != null)
            {
                // if the password within the database matches that posted via ViewModel
                if (matchedUser.Password == crypto.Compute(userSecret, matchedUser.PasswordSalt))
                {
                    // the password has been validated
                    valid = true;
                }
                else
                {
                    // adding error message to modelstate to be returned to view (arg 1 could link the error to a particular property of the model if we wanted)
                    ModelState.AddModelError("", "Username of password is not correct");
                }
            }

            return valid;
        }

        // generate passcode - also needs to be moved to service layer
        private string _NewMachineSecret()
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int secretSize = 12;
            Random rnd = new Random();

            char[] secret = new char[secretSize];

            for(int i = 0; i < secretSize; i++)
            {
                secret[i] = chars[rnd.Next(chars.Length)];
            }

            return new string(secret);
        }

    }
}