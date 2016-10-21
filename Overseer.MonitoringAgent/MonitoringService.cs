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
        MonAgentConfig Config;
        ServerCommunicator Server;

        public MonitoringService()
        {
            logFilePath = @"C:\MonitoringAgentTest\Log.txt";

            InitializeComponent();
        }

        protected async override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            TempLogger("About to read from app.config..");

            Config = new MonAgentConfig(ConfigurationManager.AppSettings["ConfigFileLocation"]);
            Config.LoadConfig();

            Server = new ServerCommunicator(Config.AppUri, Config.MachineGuid, Config.MachineSecret);

            // request bearer token
            TempLogger("Requesting Bearer Token.");
            await Server.RequestBearerToken();
            TempLogger("Token: " + Server.token);

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
                string apiResponse = Server.GetMonitoringSettingsFromApi().Result;

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

        // stop the service
        private void StopService()
        {
            TempLogger("Stopping Service.");
            ServiceController thisService = new ServiceController("OverseerMonitoringAgent");
            thisService.Stop();
        }
    }
}
