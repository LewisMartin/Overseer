using System;
using System.IO;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using Newtonsoft.Json;
using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.MonitoringClasses;
using Overseer.MonitoringAgent.Helpers;

namespace Overseer.MonitoringAgent
{
    partial class MonitoringService : ServiceBase
    {
        private Timer MonitoringUpdateSchedular;
        private MonAgentConfig Config;
        private ServerCommunicator Server;
        private OverseerMonitor Overseer;
        private Logger _Logger;

        public MonitoringService()
        {
            InitializeComponent();
        }

        protected async override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            _Logger = Logger.Instance();

            _Logger.Log("About to read from app.config..");

            Config = new MonAgentConfig(ConfigurationManager.AppSettings["ConfigFileLocation"]);
            Config.LoadConfig();

            Server = new ServerCommunicator(Config.AppUri, Config.MachineGuid, Config.MachineSecret);

            Overseer = new OverseerMonitor();

            // request bearer token
            await Server.RequestBearerToken();

            // begin scheduling
            ScheduleMonitoringUpdate();
        }

        protected override void OnStop()
        {
            _Logger.Log("The service is stopping.");
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
                    _Logger.Log("Monitoring currently enabled.");

                    DateTime scheduledTimeUtc = monitoringSettings.NextScheduledUpdate;

                    _Logger.Log("Next Scheduled time (UTC): " + scheduledTimeUtc);
                    DateTime scheduledTimeLocal = scheduledTimeUtc.ToLocalTime();
                    _Logger.Log("Next Scheduled time (Local): " + scheduledTimeLocal);
                    DateTime currentTime = DateTime.Now;
                    _Logger.Log("Current Time (Local): " + currentTime);

                    TimeSpan timeDifference = scheduledTimeLocal - currentTime;
                    int nextUpdateDue = (int)timeDifference.TotalMilliseconds;

                    _Logger.Log("Milliseconds to next update: " + nextUpdateDue);

                    if (nextUpdateDue <= 0)
                    {
                        // we've taken too long - need to get the next schedule time
                        _Logger.Log("Update opportunity expired, rescheduling from server..");
                        ScheduleMonitoringUpdate();
                    }
                    else if (nextUpdateDue > 3600000)
                    {
                        // log the exception here using logger class
                        _Logger.Log("Error during timer scheduling.");
                        StopService();
                    }
                    else
                    {
                        // update the timer with the next duetime
                        _Logger.Log("Starting countdown timer to update..");
                        MonitoringUpdateSchedular.Change(nextUpdateDue, Timeout.Infinite);
                    }
                }
                else
                {
                    _Logger.Log("Monitoring not enabled at environment level for this machine.");
                    StopService();
                }
            }
            catch (Exception e)
            {
                // log the exception here using logger class
                _Logger.Log("Error during timer scheduling: " + e.Message);
                StopService();
            }
        }

        // get monitoring info, package & send
        private void MonitoringUpdate(Object e)
        {
            // output somethnig to log for testing purposes
            _Logger.Log("~~~ Monitoring Update Beginning ~~~");

            Overseer.Snapshot();

            // output somethnig to log for testing purposes
            _Logger.Log("~~~ Monitoring Update Executed Successfully ~~~");

            // reschedule for next update
            ScheduleMonitoringUpdate();
        }

        // stop the service
        private void StopService()
        {
            _Logger.Log("Stopping Service.");
            ServiceController thisService = new ServiceController("OverseerMonitoringAgent");
            thisService.Stop();
        }
    }
}
