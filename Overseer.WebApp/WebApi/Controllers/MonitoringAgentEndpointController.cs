using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.DTOs.MonitoringAgent;
using Overseer.WebApp.WebApi.Controllers;

namespace Overseer.WebApp.WebApi
{
    [System.Web.Http.Authorize]
    public class MonitoringAgentEndpointController : BaseApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: get the monitoring interval to use within monitoring agent for specified target machine
        [HttpGet]
        public MonitoringScheduleResponse GetMonitoringScheduleSettings(Guid machineId)
        {
            Machine targetMachine = _unitOfWork.Machines.Get(machineId);

            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(targetMachine.ParentEnv);

            int interval = (int)testEnv.MonitoringSettings.MonitoringUpdateInterval;

            DateTime currentTime = DateTime.UtcNow;
            DateTime scheduledTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0);

            scheduledTime = scheduledTime.AddMinutes((currentTime.Minute % interval == 0) ? currentTime.Minute + interval : ((currentTime.Minute + interval - 1) / interval) * interval);

            return new MonitoringScheduleResponse()
            {
                MonitoringEnabled = testEnv.MonitoringSettings.MonitoringEnabled,
                NextScheduledUpdate = scheduledTime
            };
        }

        // GET: get the monitoring settings for particular machine
        
        // POST: endpoint for posting monitoring data to
        [HttpPost]
        public HttpResponseMessage SubmitMonitoringData([FromBody] string monitoringData)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("Monitoring data submitted!")
            };
        }

        // POST: Test posting of data
        [HttpPost]
        public HttpResponseMessage AddOS([FromBody] string newOSAsJson)
        {
            NewOS newOsData = JsonConvert.DeserializeObject<NewOS>(newOSAsJson);

            OperatingSys newOs = new OperatingSys()
            {
                OSName = newOsData.NewOSName,
                Bitness = newOsData.NewOSBitness
            };

            _unitOfWork.OperatingSystems.Add(newOs);

            _unitOfWork.Save();

            HttpResponseMessage response = new HttpResponseMessage()
            {
                Content = new StringContent("'" + newOsData.NewOSName + "' has been added to the OS Database!")
            };

            return response;
        }
    }
}