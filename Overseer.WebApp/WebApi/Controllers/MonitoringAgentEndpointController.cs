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
using Newtonsoft.Json.Converters;

namespace Overseer.WebApp.WebApi
{
    [CustomAPIAuth]
    public class MonitoringAgentEndpointController : BaseApiController
    {
        // GET: get the monitoring interval to use within monitoring agent for specified target machine
        [HttpGet]
        public MonitoringScheduleResponse GetMonitoringSchedule(Guid machineId)
        {
            Machine targetMachine = _unitOfWork.Machines.Get(machineId);
            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(targetMachine.ParentEnv);

            int interval = (int)testEnv.MonitoringSettings.MonitoringUpdateInterval;    // get user defined monitoring interval

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

            IEnumerable<ProcessSettings> procs = _unitOfWork.ProcessMonitoringSettings.GetByMachine(machineId);
            IEnumerable<EventLogSettings> logs = _unitOfWork.EventLogMonitoringSettings.GetByMachine(machineId);
            IEnumerable<ServiceSettings> services = _unitOfWork.ServiceMonitoringSettings.GetByMachine(machineId);

            if (procs != null)
            {
                foreach (ProcessSettings proc in procs)
                {
                    monitoringSettingsResponse.MonitoredProcessNames.Add(proc.ProcessName);
                }
            }

            if (logs != null)
            {
                foreach (EventLogSettings log in logs)
                {
                    monitoringSettingsResponse.MonitoredEventLogNames.Add(log.EventLogName);
                }
            }

            if (services != null)
            {
                foreach (ServiceSettings service in services)
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
            MonitoringData monData = JsonConvert.DeserializeObject<MonitoringData>(monitoringData, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" }); // specify format for deserialization of any dates within data
            Guid machineId = new Guid(Request.Headers.GetValues("TargetMachine").FirstOrDefault());

            try
            {
                UpdateMonitoringData(machineId, monData);
                UpdateMonitoringAlerts(machineId, monData);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("Submission of monitoring data failed - Exception:" + ex.Message)
                };
            }
        
            return new HttpResponseMessage()
            {
                Content = new StringContent("Monitoring data submitted for: " + monData.SystemInfo.MachineName)
            };
        }

        private void UpdateMonitoringData(Guid machineId, MonitoringData monData)
        {
            Machine machine = _unitOfWork.Machines.Get(machineId);
            machine.LastSnapshot = monData.SnapshotTime;

            // updating monitored data within respective tables
            UpdateSystemInformation(machineId, monData.SystemInfo);
            UpdatePerformanceInformation(machineId, monData.PerformanceInfo, monData.SnapshotTime);
            UpdateDiskInformation(machineId, monData.DiskInfo);
            UpdateProcessInformation(machineId, monData.ProcessInfo);
            UpdateEventLogInformation(machineId, monData.EventLogInfo);
            UpdateServiceInformation(machineId, monData.ServiceInfo);

            _unitOfWork.Save();
        }

        private void UpdateMonitoringAlerts(Guid machineId, MonitoringData monData)
        {
            UpdateHistoricalAlerts(machineId);
            _unitOfWork.MonitoringAlerts.AddRange(PerformAlertChecking(machineId, monData));
            _unitOfWork.Save();
        }

        // methods for updating monitoring data
        private void UpdateSystemInformation(Guid machineId, SystemInformation updatedSysInfo)
        {
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
            sysInfo.MachineName = updatedSysInfo.MachineName;
            sysInfo.IPAddress = updatedSysInfo.IPAddress;
            sysInfo.OSName = updatedSysInfo.OSName;
            sysInfo.OSNameFriendly = updatedSysInfo.OSNameFriendly;
            sysInfo.OSBitness = updatedSysInfo.OSBitness;
            sysInfo.ProcessorCount = updatedSysInfo.ProcessorCount;
            sysInfo.TotalMem = updatedSysInfo.TotalMem;
        }

        private void UpdatePerformanceInformation(Guid machineId, PerformanceInformation updatedPerfInfo, DateTime snapshotTime)
        {
            List<PerformanceInfo> perfReadings = _unitOfWork.PerformanceMonitoring.GetOrderedReadingsForMachine(machineId);

            if (perfReadings.Count >= 5)
                _unitOfWork.PerformanceMonitoring.Delete(perfReadings[perfReadings.Count - 1]);     // delete earliest reading found

            _unitOfWork.PerformanceMonitoring.Add(new PerformanceInfo()     // add latest reading
            {
                MachineID = machineId,
                ReadingDateTime = snapshotTime,
                CpuUtil = updatedPerfInfo.AvgCpuUtil,
                MemUtil = updatedPerfInfo.AvgMemUtil,
                HighCpuUtilIndicator = updatedPerfInfo.HighCpuUtilIndicator,
                HighMemUtilIndicator = updatedPerfInfo.HighMemUtilIndicator,
                TotalProcesses = updatedPerfInfo.TotalNumProcesses
            });
        }

        private void UpdateDiskInformation(Guid machineId, DiskInformation updatedDiskInfo)
        {
            _unitOfWork.DiskMonitoring.DeleteByMachine(machineId);
            foreach (SingleDrive drive in updatedDiskInfo.Drives)
            {
                _unitOfWork.DiskMonitoring.Add(new DiskInfo()
                {
                    MachineID = machineId,
                    DriveLetter = drive.Letter,
                    VolumeLabel = drive.VolumeLabel,
                    DriveType = drive.DriveType,
                    DriveFormat = drive.DriveFormat,
                    TotalSpace = drive.TotalSpace,
                    FreeSpace = drive.FreeSpace,
                    UsedSpace = drive.TotalSpace - drive.FreeSpace
                });
            }
        }

        private void UpdateProcessInformation(Guid machineId, ProcessInformation updatedProcInfo)
        {
            _unitOfWork.ProcessMonitoring.DeleteByMachine(machineId);
            foreach (SingleProc proc in updatedProcInfo.Processes)
            {
                _unitOfWork.ProcessMonitoring.Add(new ProcessInfo()
                {
                    MachineID = machineId,
                    PID = proc.Pid,
                    ProcessName = proc.Name,
                    Status = proc.Status,
                    StartTime = proc.StartTime,
                    CpuTime = proc.CpuTime,
                    ThreadCount = proc.ThreadCount,
                    WorkingSet = proc.WorkingSet,
                    PrivateBytes = proc.PrivateBytes,
                    VirtualBytes = proc.VirtualBytes
                });
            }
        }

        private void UpdateEventLogInformation(Guid machineId, EventLogInformation updatedEventLogInfo)
        {
            _unitOfWork.EventLogMonitoring.DeleteByMachine(machineId);
            foreach (SingleLog log in updatedEventLogInfo.EventLogs)
            {
                _unitOfWork.EventLogMonitoring.Add(new EventLogInfo()
                {
                    MachineID = machineId,
                    EventLogName = log.Name,
                    FriendlyLogName = log.DisplayName,
                    Exists = log.Exists,
                    TotalEvents = log.EntryTotal,
                    NumInfos = log.InfoTotal,
                    NumWarnings = log.WarningTotal,
                    NumErrors = log.ErrorTotal,
                });
            }
        }

        private void UpdateServiceInformation(Guid machineId, ServiceInformation updatedServiceInfo)
        {
            _unitOfWork.ServiceMonitoring.DeleteByMachine(machineId);
            foreach (SingleService service in updatedServiceInfo.Services)
            {
                _unitOfWork.ServiceMonitoring.Add(new ServiceInfo()
                {
                    MachineID = machineId,
                    ServiceName = service.Name,
                    Exists = service.Exists,
                    Status = service.Status,
                    StartupType = service.StartUpType
                });
            }
        }

        // methods for updating monitoring alerts based on submitted data
        private void UpdateHistoricalAlerts(Guid machineId)
        {
            var currentAlerts = _unitOfWork.MonitoringAlerts.GetAllAlertsByMachine(machineId);

            foreach (MonitoringAlert alert in currentAlerts)
            {
                alert.Historical = true;
            }

            _unitOfWork.Save();
        }

        private List<MonitoringAlert> PerformAlertChecking(Guid machineId, MonitoringData data)
        {
            List<MonitoringAlert> monitoringAlerts = new List<MonitoringAlert>();

            var perfSettings = _unitOfWork.PerformanceMonitoringSettings.Get(machineId);
            var diskSettings = _unitOfWork.DiskMonitoringSettings.Get(machineId);
            var processSettings = _unitOfWork.ProcessMonitoringSettings.GetByMachine(machineId);
            var eventlogSettings = _unitOfWork.EventLogMonitoringSettings.GetByMachine(machineId);
            var serviceSettings = _unitOfWork.ServiceMonitoringSettings.GetByMachine(machineId);

            if(perfSettings != null)
                monitoringAlerts.AddRange(PerformanceAlertChecking(machineId, data.PerformanceInfo, perfSettings));
            if(diskSettings != null)
                monitoringAlerts.AddRange(DiskAlertChecking(machineId, data.DiskInfo, diskSettings));
            if(processSettings != null)
                monitoringAlerts.AddRange(ProcessAlertChecking(machineId, data.ProcessInfo, processSettings.ToList()));
            if(eventlogSettings != null)
                monitoringAlerts.AddRange(EventLogAlertChecking(machineId, data.EventLogInfo, eventlogSettings.ToList()));
            if(serviceSettings != null)
                monitoringAlerts.AddRange(ServiceAlertChecking(machineId, data.ServiceInfo, serviceSettings.ToList()));

            return monitoringAlerts;
        }

        private List<MonitoringAlert> PerformanceAlertChecking(Guid machineId, PerformanceInformation info, PerformanceSettings settings)
        {
            List<MonitoringAlert> perfAlerts = new List<MonitoringAlert>();

            if (settings.AvgCpuUtilAlertsOn)
            {
                if (info.AvgCpuUtil >= settings.AvgCpuUtilAlertValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Cpu", 1, "AvgCpuUtil", info.AvgCpuUtil.ToString()));
                else if (info.AvgCpuUtil >= settings.AvgCpuUtilWarnValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Cpu", 0, "AvgCpuUtil", info.AvgCpuUtil.ToString()));
            }

            if (settings.CpuHighUtilAlertsOn)
            {
                if (info.HighCpuUtilIndicator >= settings.CpuHighUtilAlertValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Cpu", 1, "CpuHighUtil", info.AvgCpuUtil.ToString()));
                else if (info.HighCpuUtilIndicator >= settings.CpuHighUtilWarnValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Cpu", 0, "CpuHighUtil", info.AvgCpuUtil.ToString()));
            }

            if (settings.AvgMemUtilAlertsOn)
            {
                if (info.AvgMemUtil >= settings.AvgMemUtilAlertValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Mem", 1, "AvgMemUtil", info.AvgMemUtil.ToString()));
                else if (info.AvgMemUtil >= settings.AvgMemUtilWarnValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Mem", 0, "AvgMemUtil", info.AvgMemUtil.ToString()));
            }

            if (settings.MemHighUtilAlertsOn)
            {
                if (info.HighMemUtilIndicator >= settings.MemHighUtilAlertsValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Mem", 1, "MemHighUtil", info.AvgMemUtil.ToString()));
                else if (info.HighMemUtilIndicator >= settings.MemHighUtilWarnValue)
                    perfAlerts.Add(CreateMonitoringAlert(machineId, 0, "Mem", 0, "MemHighUtil", info.AvgMemUtil.ToString()));
            }

            return perfAlerts;
        }

        private List<MonitoringAlert> DiskAlertChecking(Guid machineId, DiskInformation info, DiskSettings settings)
        {
            List<MonitoringAlert> diskAlerts = new List<MonitoringAlert>();

            if(settings.UsedSpaceAlertsOn)
            {
                foreach (SingleDrive drive in info.Drives)
                {
                    if ((100 - ((drive.FreeSpace / drive.TotalSpace) * 100)) >= settings.UsedSpaceAlertValue)
                        diskAlerts.Add(CreateMonitoringAlert(machineId, 1, drive.Letter, 1, "UsedSpace", (100 - ((drive.FreeSpace / drive.TotalSpace) * 100)).ToString()));
                    else if ((100 - ((drive.FreeSpace / drive.TotalSpace) * 100)) >= settings.UsedSpaceWarningValue)
                        diskAlerts.Add(CreateMonitoringAlert(machineId, 1, drive.Letter, 0, "UsedSpace", (100 - ((drive.FreeSpace / drive.TotalSpace) * 100)).ToString()));
                }
            }
                
            return diskAlerts;
        }

        private List<MonitoringAlert> ProcessAlertChecking(Guid machineId, ProcessInformation info, List<ProcessSettings> settings)
        {
            List<MonitoringAlert> procAlerts = new List<MonitoringAlert>();

            foreach (SingleProc proc in info.Processes)
            {
                ProcessSettings procSet = settings.Find(s => s.ProcessName == proc.Name);

                if (procSet.WorkingSetAlertsOn)
                {
                    if ((proc.WorkingSet/1024) >= procSet.WSAlertValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 1, "WorkingSet", (proc.WorkingSet/1024).ToString() + " kb"));
                    else if ((proc.WorkingSet/1024) >= procSet.WSWarnValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 0, "WorkingSet", (proc.WorkingSet/1024).ToString() + " kb"));
                }

                if (procSet.PrivateBytesAlertsOn)
                {
                    if((proc.PrivateBytes/1024) >= procSet.PBAlertValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 1, "PrivateBytes", (proc.PrivateBytes/1024).ToString() + " kb"));
                    else if((proc.PrivateBytes/1024) >= procSet.PBWarnValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 0, "PrivateBytes", (proc.PrivateBytes/1024).ToString() + " kb"));
                }

                if (procSet.VirtualBytesAlertsOn)
                {
                    if((proc.VirtualBytes/1024) >= procSet.VBAlertValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 1, "VirtualBytes", (proc.VirtualBytes/1024).ToString() + " kb"));
                    else if((proc.VirtualBytes/1024) >= procSet.VBWarnValue)
                        procAlerts.Add(CreateMonitoringAlert(machineId, 2, proc.Name, 0, "VirtualBytes", (proc.VirtualBytes/1024).ToString() + " kb"));
                }
            }

            return procAlerts;
        }

        private List<MonitoringAlert> EventLogAlertChecking(Guid machineId, EventLogInformation info, List<EventLogSettings> settings)
        {
            List<MonitoringAlert> logAlerts = new List<MonitoringAlert>();

            foreach (SingleLog log in info.EventLogs)
            {
                EventLogSettings logSet = settings.Find(s => s.EventLogName == log.Name);

                if (log.Exists)
                {
                    if (logSet.ErrorCountAlertsOn)
                    {
                        if (log.ErrorTotal >= logSet.ErrorCountAlertValue)
                            logAlerts.Add(CreateMonitoringAlert(machineId, 3, log.Name, 1, "ErrorCount", (log.ErrorTotal).ToString()));
                        else if (log.ErrorTotal >= logSet.ErrorCountWarnValue)
                            logAlerts.Add(CreateMonitoringAlert(machineId, 3, log.Name, 0, "ErrorCount", (log.ErrorTotal).ToString()));
                    }

                    if (logSet.WarningCountAlertsOn)
                    {
                        if (log.WarningTotal >= logSet.WarningCountAlertValue)
                            logAlerts.Add(CreateMonitoringAlert(machineId, 3, log.Name, 1, "WarningCount", (log.WarningTotal).ToString()));
                        else if (log.WarningTotal >= logSet.WarningCountAlertValue)
                            logAlerts.Add(CreateMonitoringAlert(machineId, 3, log.Name, 0, "WarningCount", (log.WarningTotal).ToString()));
                    }
                }
                else
                {
                    if (logSet.NotFoundAlertsOn)
                        logAlerts.Add(CreateMonitoringAlert(machineId, 3, log.Name, (int)logSet.NotFoundSeverity, "EventLogNotFound", ""));
                }
            }

            return logAlerts;
        }

        private List<MonitoringAlert> ServiceAlertChecking(Guid machineId, ServiceInformation info, List<ServiceSettings> settings)
        {
            List<MonitoringAlert> serviceAlerts = new List<MonitoringAlert>();

            foreach(SingleService service in info.Services)
            {
                ServiceSettings serviceSet = settings.Find(s => s.ServiceName == service.Name);

                if (service.Exists)
                {
                    if (serviceSet.NotRunningAlertsOn)
                        serviceAlerts.Add(CreateMonitoringAlert(machineId, 4, service.Name, (int)serviceSet.NotRunningSeverity, "ServiceNotRunning", "True"));
                }
                else
                {
                    if (serviceSet.NotFoundAlertsOn)
                        serviceAlerts.Add(CreateMonitoringAlert(machineId, 4, service.Name, (int)serviceSet.NotFoundSeverity, "ServiceNotFound", "True"));
                }
            }

            return serviceAlerts;
        }

        private MonitoringAlert CreateMonitoringAlert(Guid machId, int cat, string src, int sev, string trigN, string trigV)
        {
            return new MonitoringAlert
            {
                MachineId = machId,
                Category = cat,
                Source = src,
                Severity = sev,
                TriggerName = trigN,
                TriggerValue = trigV,
                AlertCreationTime = DateTime.Now.ToLocalTime(),
                Historical = false,
                Archived = false
            };
        }
    }
}