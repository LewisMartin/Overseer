using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Overseer.DTOs.MonitoringAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.Helpers
{
    class ServerCommunicator
    {
        public string OverseerAuthEndpoint { get; set; }
        public string OverseerMonitoringScheduleEndpoint { get; set; }
        public string OverseerMonitoringSettingsEndpoint { get; set; }
        public string OverseerMonitoringDataEndpoint { get; set; }
        public string MachineGuid { get; set; }
        public string MachineSecret { get; set; }

        private Logger _Logger;

        // constructor
        public ServerCommunicator(string serverAddress, string machineGuid, string machineSecret)
        {
            MachineGuid = machineGuid;
            MachineSecret = machineSecret;

            OverseerAuthEndpoint = serverAddress;
            OverseerMonitoringScheduleEndpoint = serverAddress + "/api/MonitoringAgentEndpoint/GetMonitoringSchedule?machineId=" + MachineGuid;
            OverseerMonitoringSettingsEndpoint = serverAddress + "/api/MonitoringAgentEndpoint/GetMonitoringSettings?machineId=" + MachineGuid;
            OverseerMonitoringDataEndpoint = serverAddress + "/api/MonitoringAgentEndpoint/SubmitMonitoringData";

            _Logger = Logger.Instance();
        }

        // GET monitoring schedule from API
        public async Task<string> GetMonitoringScheduleFromApi()
        {
            _Logger.Log("Requesting Monitoring Schedule..");

            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("TargetMachine", MachineGuid.ToString());
                httpClient.DefaultRequestHeaders.Add("TargetSecret", MachineSecret);

                // making the request
                HttpResponseMessage response = await httpClient.GetAsync(OverseerMonitoringScheduleEndpoint);

                // return the content of the response as a string
                return ReadResponseMessage(response).Result;
            }
        }

        // GET monitoring settings from API
        public async Task<string> GetMonitoringSettingsFromApi()
        {
            _Logger.Log("Requesting Monitoring Settings");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("TargetMachine", MachineGuid.ToString());
                httpClient.DefaultRequestHeaders.Add("TargetSecret", MachineSecret);

                HttpResponseMessage response = await httpClient.GetAsync(OverseerMonitoringSettingsEndpoint);

                return ReadResponseMessage(response).Result;
            }
        }

        // POST monitoring data to api
        public async Task<string> SubmitMonitoringData(MonitoringData monData)
        {
            // serializing monitoring data to json string
            string jsonData = string.Format("={0}", JsonConvert.SerializeObject(monData));

            // forming the content of the psot request
            var postContent = new StringContent(
                jsonData,
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("TargetMachine", MachineGuid.ToString());
                httpClient.DefaultRequestHeaders.Add("TargetSecret", MachineSecret);

                // making the request - posting the data
                HttpResponseMessage response = await httpClient.PostAsync(OverseerMonitoringDataEndpoint, postContent);

                // return the content of the response as a string
                return ReadResponseMessage(response).Result;
            }
        }

        private async Task<string> ReadResponseMessage(HttpResponseMessage response)
        {
            if (response.StatusCode.ToString() == "OK")
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Error " + response.StatusCode.ToString() + ": " + response.ReasonPhrase; 
            }
        }
    }
}
