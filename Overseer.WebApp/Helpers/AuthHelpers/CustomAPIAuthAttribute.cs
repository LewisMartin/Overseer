using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Overseer.WebApp.Helpers.AuthHelpers
{
    public class CustomAPIAuthAttribute : AuthorizeAttribute
    {
        protected IUnitOfWork _unitOfWork;

        public CustomAPIAuthAttribute()
        {
            // note, we can't assign the unit of work here - authorize attributes are only instantiated once (therefore context isn't refreshed and continually returns old data)
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Guid TargetMachineGuid = new Guid(actionContext.Request.Headers.GetValues("TargetMachine").FirstOrDefault());
            string TargetSecret = actionContext.Request.Headers.GetValues("TargetSecret").FirstOrDefault();

            // check existence of machine
            if (CheckMachineExistence(TargetMachineGuid))
            {
                if (AuthorizeMonitoringAgentRequest(TargetMachineGuid, TargetSecret))
                {
                    return;
                }
                else
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                actionContext.Response.Content = new System.Net.Http.StringContent("No machine found that matches Guid: " + TargetMachineGuid);
            }
        }

        // check existence of machine
        private bool CheckMachineExistence(Guid machineGuid)
        {
            // instantiate it here instead
            _unitOfWork = new UnitOfWork(new OverseerDBContext());

            var machine = _unitOfWork.Machines.Get(machineGuid);

            if (machine != null)
                return true;
            else
                return false;
        }

        // perform authorization on machine credential passed with request
        private bool AuthorizeMonitoringAgentRequest(Guid machineGuid, string machineSecret)
        {
            // instantiate it here instead
            _unitOfWork = new UnitOfWork(new OverseerDBContext());

            var creds = _unitOfWork.MonitoringAgentCredentials.Get(machineGuid);

            if (creds == null)
            {
                return false;
            }
            else if (creds.MonitoringAgentSecret == machineSecret)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}