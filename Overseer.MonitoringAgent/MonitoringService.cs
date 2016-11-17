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
        private Timer _MonitoringUpdateSchedular;
        private MonAgentConfig _Config;
        private ServerCommunicator _Server;
        private SystemMonitoring _SysMonitoring;
        private Logger _Logger;

        public MonitoringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            _Logger = Logger.Instance();

            _Logger.Log("About to read from app.config..");

            _Config = new MonAgentConfig(ConfigurationManager.AppSettings["ConfigFileLocation"]);
            _Config.LoadConfig();

            _Server = new ServerCommunicator(_Config.AppUri, _Config.MachineGuid, _Config.MachineSecret);

            _SysMonitoring = new SystemMonitoring();

            // review monitoring settings for process/service/eventlog monitoring
            ReviewMonitoringSettings();

            // begin scheduling
            ScheduleMonitoringUpdate();
        }

        protected override void OnStop()
        {
            _Logger.Log("The service is stopping.");
            this._MonitoringUpdateSchedular.Dispose();
        }

        private void ScheduleMonitoringUpdate()
        {
            // define the callback for the timer
            _MonitoringUpdateSchedular = new Timer(new TimerCallback(MonitoringUpdate));

            string apiResponse = "";

            try
            {
                // make get request to server for monitoring settings for this machine
                apiResponse = _Server.GetMonitoringScheduleFromApi().Result;

                MonitoringScheduleResponse monitoringSchedule = JsonConvert.DeserializeObject<MonitoringScheduleResponse>(apiResponse);

                if (monitoringSchedule.MonitoringEnabled)
                {
                    _Logger.Log("Monitoring currently enabled.");

                    DateTime scheduledTimeUtc = monitoringSchedule.NextScheduledUpdate;

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
                        _MonitoringUpdateSchedular.Change(nextUpdateDue, Timeout.Infinite);
                    }
                }
                else
                {
                    _Logger.Log("Monitoring not enabled at environment level for this machine.");
                    StopService();
                }
            }
            catch (JsonException)
            {
                _Logger.Log("Server Response Error: " + apiResponse);
                StopService();
            }
            catch (Exception e)
            {
                _Logger.Log("Error during timer scheduling: " + e.Message);
                StopService();
            }
        }

        // get monitoring info, package & send
        private async void MonitoringUpdate(Object e)
        {
            _Logger.Log("~~~ Monitoring Update Beginning ~~~");

            _SysMonitoring.TakeSnapshot();

            _Logger.Log("~~~ Monitoring Update Executed Successfully ~~~");

            _Logger.Log("Attempting monitoring data submission..");

            try
            {
                _Logger.Log(await _Server.SubmitMonitoringData(_SysMonitoring.GenerateMonitoringDataDTO()));
            }
            catch (Exception serverEx)
            {
                _Logger.Log("Error Occured whilst submitting monitoring data: " + serverEx.Message);
            }

            // review monitoring settings for process/service/eventlog monitoring
            ReviewMonitoringSettings();

            // reschedule for next update
            ScheduleMonitoringUpdate();
        }

        // stop the service
        private void StopService()
        {
            _Logger.Log("Stopping Service.");

            this.Stop();
        }

        private void ReviewMonitoringSettings()
        {
            _Logger.Log("Reviewing monitoring settings.");

            string apiResponse = "";
            try
            {
                // make get request to server for monitoring settings for this machine
                apiResponse = _Server.GetMonitoringSettingsFromApi().Result;
                MonitoringSettingsResponse settings = JsonConvert.DeserializeObject<MonitoringSettingsResponse>(apiResponse);
                _SysMonitoring.UpdateMonitoringSettings(settings.MonitoredProcessNames, settings.MonitoredEventLogNames, settings.MonitoredServiceNames);
            }
            catch (JsonException)
            {
                _Logger.Log("Server Response Error: " + apiResponse);
                StopService();
            }
            catch (Exception e)
            {
                _Logger.Log("Error during monitoring settings review: " + e.Message);
                StopService();
            }
        }
    }
}
