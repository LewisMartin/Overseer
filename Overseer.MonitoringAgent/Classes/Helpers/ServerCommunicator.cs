using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent
{
    class ServerCommunicator
    {
        public string OverseerAuthEndpoint { get; set; }
        public string OverseerMonitoringSettingsEndpoint { get; set; }
        public string OverseerMonitoringDataEndpoint { get; set; }
        public string MachineGuid { get; set; }
        public string MachineSecret { get; set; }
        public string token { get; set; }

        // constructor
        public ServerCommunicator(string serverAddress, string machineGuid, string machineSecret)
        {
            MachineGuid = machineGuid;
            MachineSecret = machineSecret;

            OverseerAuthEndpoint = serverAddress;
            OverseerMonitoringSettingsEndpoint = serverAddress + "/api/MonitoringAgentEndpoint/GetMonitoringScheduleSettings?machineId=" + MachineGuid;
            OverseerMonitoringDataEndpoint = serverAddress + "/api/SubmitMonitoringData";
        }

        // method to request bearer token
        public async Task RequestBearerToken()
        {
            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.BaseAddress = new Uri(OverseerAuthEndpoint);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // setting the form content of the request
                var reqFormContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", MachineGuid),
                    new KeyValuePair<string, string>("client_secret", MachineSecret)
                });

                // initiate the request
                HttpResponseMessage response = await httpClient.PostAsync("/Token", reqFormContent);

                // get bearer token from the body of the response
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                string bearerToken = jObject.GetValue("access_token").ToString();

                token = bearerToken;
            }
        }

        // get monitoring settings from api
        public async Task<string> GetMonitoringSettingsFromApi()
        {
            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                // making the request
                HttpResponseMessage response = await httpClient.GetAsync(OverseerMonitoringSettingsEndpoint);
                string responseString = await response.Content.ReadAsStringAsync();

                // return the content of the response as a string
                return responseString;
            }
        }
    }
}
