using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.DTOs.MonitoringAgent;
using Overseer.WebApp.WebApi.Controllers;

namespace Overseer.WebApp.WebApi
{
    [CustomAPIAuth]
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
        [HttpGet]
        public MonitoringSettingsResponse GetMonitoringSettings(Guid machineId)
        {
            MonitoringSettingsResponse monitoringSettingsResponse = new MonitoringSettingsResponse();

            IEnumerable<ProcessInfo> procs = _unitOfWork.ProcessMonitoring.GetByMachine(machineId);
            IEnumerable<EventLogInfo> logs = _unitOfWork.EventLogMonitoring.GetByMachine(machineId);
            IEnumerable<ServiceInfo> services = _unitOfWork.ServiceMonitoring.GetByMachine(machineId);

            if (procs != null)
            {
                foreach (ProcessInfo proc in procs)
                {
                    monitoringSettingsResponse.MonitoredProcessNames.Add(proc.ProcessName);
                }
            }

            if (logs != null)
            {
                foreach (EventLogInfo log in logs)
                {
                    monitoringSettingsResponse.MonitoredEventLogNames.Add(log.EventLogName);
                }
            }

            if (services != null)
            {
                foreach (ServiceInfo service in services)
                {
                    monitoringSettingsResponse.MonitoredServiceNames.Add(service.ServiceName);
                }
            }

            return monitoringSettingsResponse;
        }
        
        // POST: endpoint for posting monitoring data to
        [HttpPost]
        public HttpResponseMessage SubmitMonitoringData([FromBody] string monitoringData)   // REFACTOR
        {
            MonitoringData monData = JsonConvert.DeserializeObject<MonitoringData>(monitoringData);

            Guid machineId = new Guid(Request.Headers.GetValues("TargetMachine").FirstOrDefault());

            // getting/creating system information record in 'SystemMonitoring' table.
            SystemInfo sysInfo = _unitOfWork.SystemInfoMonitoring.Get(machineId);

            if (sysInfo == null)
            {
                _unitOfWork.SystemInfoMonitoring.Add(new SystemInfo()
                {
                    MachineID = machineId
                });

                _unitOfWork.Save();

                sysInfo = _unitOfWork.SystemInfoMonitoring.Get(machineId);
            }

            // getting/creating system information record in 'PerformanceMonitoring' table.
            PerformanceInfo perfInfo = _unitOfWork.PerformanceMonitoring.Get(machineId);

            if (perfInfo == null)
            {
                _unitOfWork.PerformanceMonitoring.Add(new PerformanceInfo()
                {
                    MachineID = machineId
                });
                _unitOfWork.Save();

                perfInfo = _unitOfWork.PerformanceMonitoring.Get(machineId);
            }

            // far less cumbersome just to delete all disk records for this machine & add new ones rather than attempting to maintain a list
            _unitOfWork.DiskMonitoring.DeleteByMachine(machineId);
            _unitOfWork.Save();

            foreach (SingleDrive drive in monData.DiskInfo.Drives)
            {
                _unitOfWork.DiskMonitoring.Add(new DiskInfo()
                {
                    MachineID = machineId,
                    DriveLetter = drive.Name,
                    VolumeLabel = drive.VolumeLabel,
                    DriveType = drive.DriveType,
                    DriveFormat = drive.DriveFormat,
                    TotalSpace = drive.TotalSpace,
                    FreeSpace = drive.FreeSpace,
                    AvailableSpace = drive.AvailableSpace
                });
            }

            sysInfo.MachineName = monData.SystemInfo.MachineName;
            sysInfo.IPAddress = monData.SystemInfo.IPAddress;
            sysInfo.OSName = monData.SystemInfo.OSName;
            sysInfo.OSNameFriendly = monData.SystemInfo.OSNameFriendly;
            sysInfo.OSBitness = monData.SystemInfo.OSBitness;
            sysInfo.ProcessorCount = monData.SystemInfo.ProcessorCount;
            sysInfo.TotalMem = monData.SystemInfo.TotalMem;

            perfInfo.CpuUtil = monData.PerformanceInfo.AvgCpuUtil;
            perfInfo.MemUtil = monData.PerformanceInfo.AvgMemUtil;
            perfInfo.HighCpuUtilIndicator = monData.PerformanceInfo.HighCpuUtilIndicator;
            perfInfo.HighMemUtilIndicator = monData.PerformanceInfo.HighMemUtilIndicator;
            perfInfo.TotalProcesses = monData.PerformanceInfo.TotalNumProcesses;

            _unitOfWork.Save();

            return new HttpResponseMessage()
            {
                Content = new StringContent("Monitoring data submitted for: " + monData.SystemInfo.MachineName)
            };
        }
    }
}