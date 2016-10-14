using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Overseer.DTOs.MonitoringAgent;

namespace Overseer.MonitoringAgent
{
    partial class MonitoringService : ServiceBase
    {
        string logFilePath;
        Timer MonitoringUpdateSchedular;
        string token;
        MonAgentConfig Config;

        public MonitoringService()
        {
            logFilePath = @"C:\MonitoringAgentTest\Log.txt";

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            TempLogger("About to read from app.config..");

            Config = new MonAgentConfig(ConfigurationManager.AppSettings["ConfigFileLocation"]);
            Config.LoadConfig();

            // get credentials for token request
            //Config.MachineGuid = ConfigurationManager.AppSettings["machineGUID"];
            //Config.MachineSecret = ConfigurationManager.AppSettings["machineSecret"];

            TempLogger("Requesting Bearer Token.");

            // request bearer token
            token = RequestBearerToken(Config.MachineGuid, Config.MachineSecret, Config.AppUri).Result;

            TempLogger("Token: " + token);

            // begin scheduling
            ScheduleMonitoringUpdate();
        }

        protected override void OnStop()
        {
            TempLogger("Service Stopped.");
            this.MonitoringUpdateSchedular.Dispose();
        }

        private void ScheduleMonitoringUpdate()
        {
            // define the callback for the timer
            MonitoringUpdateSchedular = new Timer(new TimerCallback(MonitoringUpdate));

            try
            {
                // make get request to server for monitoring settings for this machine
                string apiResponse = GetMonitoringSettingsFromApi(token, Config.AppUri + "/api/MonitoringAgentEndpoint/GetMonitoringScheduleSettings?machineId=" + Config.MachineGuid).Result;

                // convert to 'MonitoringScheduleResponse' object so that we can grab the data inside
                MonitoringScheduleResponse monitoringSettings = JsonConvert.DeserializeObject<MonitoringScheduleResponse>(apiResponse);

                if (monitoringSettings.MonitoringEnabled)
                {
                    TempLogger("Monitoring currently enabled.");

                    DateTime scheduledTimeUtc = monitoringSettings.NextScheduledUpdate;

                    TempLogger("Next Scheduled time (UTC): " + scheduledTimeUtc);
                    DateTime scheduledTimeLocal = scheduledTimeUtc.ToLocalTime();
                    TempLogger("Next Scheduled time (Local): " + scheduledTimeLocal);
                    DateTime currentTime = DateTime.Now;
                    TempLogger("Current Time (Local): " + currentTime);

                    TimeSpan timeDifference = scheduledTimeLocal - currentTime;
                    int nextUpdateDue = (int)timeDifference.TotalMilliseconds;

                    TempLogger("Milliseconds to next update: " + nextUpdateDue);

                    if (nextUpdateDue <= 0)
                    {
                        // we've taken too long - need to get the next schedule time
                        TempLogger("Update opportunity expired, rescheduling from server..");
                        ScheduleMonitoringUpdate();
                    }
                    else if (nextUpdateDue > 3600000)
                    {
                        // log the exception here using logger class
                        TempLogger("Error during timer scheduling.");
                        StopService();
                    }
                    else
                    {
                        // update the timer with the next duetime
                        TempLogger("Starting countdown timer to update..");
                        MonitoringUpdateSchedular.Change(nextUpdateDue, Timeout.Infinite);
                    }
                }
                else
                {
                    TempLogger("Monitoring not enabled at environment level for this machine.");
                    StopService();
                }
            }
            catch (Exception e)
            {
                // log the exception here using logger class
                TempLogger("Error during timer scheduling: " + e.Message);
                StopService();
            }
        }

        // get monitoring info, package & send
        private void MonitoringUpdate(Object e)
        {
            // output somethnig to log for testing purposes
            TempLogger("~~~ Monitoring Update Executed Successfully ~~~");

            // reschedule for next update
            ScheduleMonitoringUpdate();
        }

        // method to request bearer token
        private async Task<string> RequestBearerToken(string cliId, string cliSecret, string appUri)
        {
            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.BaseAddress = new Uri(appUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // setting the form content of the request
                var reqFormContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", cliId),
                    new KeyValuePair<string, string>("client_secret", cliSecret)
                });

                // initiate the request
                HttpResponseMessage response = await httpClient.PostAsync("/Token", reqFormContent);

                // get bearer token from the body of the response
                var responseJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                string bearerToken = jObject.GetValue("access_token").ToString();

                // return the bearer token
                return bearerToken;
            }
        }

        // get monitoring settings from api
        private static async Task<string> GetMonitoringSettingsFromApi(string token, string apiUrl)
        {
            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                // making the request
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                string responseString = await response.Content.ReadAsStringAsync();

                // return the content of the response as a string
                return responseString;
            }
        }

        // temporary logging method
        private void TempLogger(string msg)
        {
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath);
                    sw.WriteLine(DateTime.Now + " | Log file created.");
                }

                sw.WriteLine(DateTime.Now + " | " + msg);

                sw.Close();
            }
        }

        // stop he service
        private void StopService()
        {
            TempLogger("Stopping Service.");
            ServiceController thisService = new ServiceController("OverseerMonitoringAgent");
            thisService.Stop();
        }
    }
}
