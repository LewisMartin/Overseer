using Newtonsoft.Json;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Machine;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;
using System.Linq;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class MachineController : BaseController
    {
        // GET: Machineseer - page showing monitoirng of specific machine
        public ActionResult Machineseer(Guid machineId)
        {
            MachineseerViewModel viewModel = new MachineseerViewModel();

            Machine machine = _unitOfWork.Machines.GetMachineseerDataMonitored(machineId);

            MonitoringSettings monSettings = _unitOfWork.MonitoringSettings.Get(machine.TestEnvironment.EnvironmentID);

            if (machine != null)
            {
                viewModel.MachineId = machine.MachineID;
                viewModel.DisplayName = machine.DisplayName;
                viewModel.ParentEnvironmentId = machine.ParentEnv;
                viewModel.MonitoringEnabled = monSettings.MonitoringEnabled;
                if (machine.LastSnapshot != null)
                {
                    DateTime dt = (DateTime)machine.LastSnapshot;
                    viewModel.LatestMonitoringUpdate = dt.ToString("MM/dd/yyyy h:mm tt");
                } 
                else
                {
                    viewModel.LatestMonitoringUpdate = "Never";
                }

                viewModel.MachineDetails = new MachineDetailsViewModel()    // Static (user-entered) system information.
                {
                    ParentEnvironmentName = machine.TestEnvironment.EnvironmentName,    // <--- eager loaded environment.
                    DisplayName = machine.DisplayName,
                    MachineName = machine.ComputerName,
                    IpAddress = machine.IPV4,
                    FQDN = machine.FQDN,
                    OperatingSysName = machine.OperatingSys.OSName,                     // <--- eager loaded.
                    OperatingSysBitness = machine.OperatingSys.Bitness,                 // <--- eager loaded.
                    NumProcessors = machine.NumProcessors,
                    TotalMemGbs = machine.TotalMemGbs
                };

                if (_unitOfWork.TestEnvironments.Get(machine.ParentEnv).Creator == GetLoggedInUserId())
                    viewModel.EditPermission = true;

                viewModel.BaseAppUrl = GetBaseApplicationUrl();
                viewModel.RefreshInterval = GetMillisecondsToNextUpdate(machine.ParentEnv);

                return View(viewModel);
            }

            return HttpNotFound();
        }

        // Partial Views for 'Machineseer' page.
        [HttpGet]
        public PartialViewResult _MonitoringSystemInfo(Guid machineId)
        {
            Machine machine = _unitOfWork.Machines.GetMachineseerDataMonitored(machineId);

            _MonitoringSystemInfoViewModel viewModel = new _MonitoringSystemInfoViewModel();

            if (machine.SystemInformationData != null)
            {
                viewModel.MachineName = machine.SystemInformationData.MachineName;
                viewModel.IPAddress = machine.SystemInformationData.IPAddress;
                viewModel.OSNameFriendly = machine.SystemInformationData.OSNameFriendly;
                viewModel.OSName = machine.SystemInformationData.OSName;
                viewModel.OSBitness = machine.SystemInformationData.OSBitness;
                viewModel.ProcessorCount = (machine.SystemInformationData.ProcessorCount != null ? (int)machine.SystemInformationData.ProcessorCount : 0);
                viewModel.TotalMem = (machine.SystemInformationData.TotalMem != null ? Convert.ToInt32(machine.SystemInformationData.TotalMem) : 0);
            }

            return PartialView(viewModel);
        }

        [HttpGet]
        public PartialViewResult _MonitoringSummary(Guid machineId)
        {
            Machine machine = _unitOfWork.Machines.GetMachineseerDataMonitored(machineId);
            PerformanceInfo latestPerfReading = _unitOfWork.PerformanceMonitoring.GetLatestReading(machineId);

            _MonitoringSummaryViewModel viewModel = new _MonitoringSummaryViewModel();

            if (machine != null)
            {

                if (latestPerfReading != null)
                {
                    viewModel.PerformanceInfo.AvgCpuUtil = (latestPerfReading.CpuUtil != null ? (float)latestPerfReading.CpuUtil : 101).ToString("0.00");
                    viewModel.PerformanceInfo.AvgMemUtil = (latestPerfReading.MemUtil != null ? (float)latestPerfReading.MemUtil : 101).ToString("0.00");
                    viewModel.PerformanceInfo.HighCpuUtilIndicator = (latestPerfReading.HighCpuUtilIndicator != null ? (float)latestPerfReading.HighCpuUtilIndicator : 0).ToString("0.00");
                    viewModel.PerformanceInfo.HighMemUtilIndicator = (latestPerfReading.HighMemUtilIndicator != null ? (float)latestPerfReading.HighMemUtilIndicator : 0).ToString("0.00");
                    viewModel.PerformanceInfo.TotalNumProcesses = (latestPerfReading.TotalProcesses != null ? (int)latestPerfReading.TotalProcesses : 0);
                }

                if (machine.PerformanceData != null)
                {
                    List<string> readingTimes = new List<string>();
                    List<float> cpuChartData = new List<float>(), memChartData = new List<float>();

                    int i = 1;
                    foreach (PerformanceInfo perfInfo in machine.PerformanceData)
                    {
                        readingTimes.Add(perfInfo.ReadingDateTime.ToString("HH:mm"));
                        cpuChartData.Add((float)perfInfo.CpuUtil);
                        memChartData.Add((float)perfInfo.MemUtil);
                        i++;
                    }

                    viewModel.PerformanceInfo.ReadingTimes = new System.Web.HtmlString(JsonConvert.SerializeObject(readingTimes, Formatting.None));
                    viewModel.PerformanceInfo.CpuChartData = new System.Web.HtmlString(JsonConvert.SerializeObject(cpuChartData, Formatting.None));
                    viewModel.PerformanceInfo.MemChartData = new System.Web.HtmlString(JsonConvert.SerializeObject(memChartData, Formatting.None));
                }

                if (machine.DiskData != null)
                {
                    foreach (DiskInfo disk in machine.DiskData)
                    {
                        viewModel.DiskInfo.Drives.Add(new SingleDriveViewModel()
                        {
                            Name = disk.DriveLetter,
                            VolumeLabel = disk.VolumeLabel,
                            DriveType = disk.DriveType,
                            DriveFormat = disk.DriveFormat,
                            TotalSpace = (decimal)disk.TotalSpace,
                            FreeSpace = (decimal)disk.FreeSpace,
                            UsedSpace = (decimal)disk.UsedSpace
                        });
                    }
                }

                if (machine.ProcessConfig != null)
                {
                    foreach (ProcessSettings setting in machine.ProcessConfig)  // each monitored process
                    {
                        List<SingleProcessViewModel> procs = new List<SingleProcessViewModel>();

                        if (machine.ProcessData != null)
                        {
                            foreach (ProcessInfo proc in machine.ProcessData)   // each process for which we have data
                            {
                                if (proc.ProcessName == setting.ProcessName)
                                {
                                    procs.Add(new SingleProcessViewModel()
                                    {
                                        PID = proc.PID,
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
                        }

                        viewModel.ProcessInfo.MonitoredProcesses.Add(new ProcessGroupViewModel() { ProcessName = setting.ProcessName, Processes = procs });

                    }
                }

                if (machine.EventLogData != null)
                {
                    foreach (EventLogInfo log in machine.EventLogData)
                    {
                        viewModel.EventLogInfo.EventLogs.Add(new SingleEventLogViewModel()
                        {
                            EventLogName = log.EventLogName,
                            FriendlyLogName = log.FriendlyLogName,
                            Exists = log.Exists,
                            TotalEvents = log.TotalEvents,
                            NumInfos = log.NumInfos,
                            NumWarnings = log.NumWarnings,
                            NumErrors = log.NumErrors
                        });
                    }
                }

                if (machine.ServiceData != null)
                {
                    foreach (ServiceInfo service in machine.ServiceData)
                    {
                        viewModel.ServiceInfo.Services.Add(new SingleServiceViewModel()
                        {
                            ServiceName = service.ServiceName,
                            Exists = service.Exists,
                            Status = service.Status,
                            StartupType = service.StartupType
                        });
                    }
                }
            }

            return PartialView(viewModel);
        }

        // MachineConfiguration - page to change machine details & configure machine level monitoring settings
        // GET: 
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpGet]
        public ActionResult MachineConfiguration(Guid machineId)
        {
            Machine machineToUpdate = _unitOfWork.Machines.GetMachineAndParent(machineId);
            int loggedInUserId = GetLoggedInUserId();

            // check if user has permission on this environment
            if (loggedInUserId != machineToUpdate.TestEnvironment.Creator)
            {
                return View("~/Views/UserAuth/Unauthorized.cshtml");
            }
            else
            {
                var operatingSystems = _unitOfWork.OperatingSystems.GetAll();
                var environments = _unitOfWork.TestEnvironments.GetEnvironmentsByCreator(loggedInUserId);

                List<SelectListItem> parentEnvOps = new List<SelectListItem>();
                List<SelectListItem> operatingSysOps = new List<SelectListItem>();
                List<SelectListItem> currentMonitoredProcs = new List<SelectListItem>();
                List<SelectListItem> currentMonitoredLogs = new List<SelectListItem>();
                List<SelectListItem> currentMonitoredServices = new List<SelectListItem>();

                foreach (var environment in environments)
                {
                    parentEnvOps.Add(new SelectListItem
                    {
                        Value = environment.EnvironmentID.ToString(),
                        Text = environment.EnvironmentName,
                        Selected = (environment.EnvironmentID == machineToUpdate.ParentEnv ? true : false)
                    });
                }

                foreach (var os in operatingSystems)
                {
                    operatingSysOps.Add(new SelectListItem
                    {
                        Value = os.OperatingSysID.ToString(),
                        Text = os.OSName,
                        Selected = (os.OperatingSysID == machineToUpdate.OperatingSys.OperatingSysID ? true : false)
                    });
                }

                IEnumerable<ProcessSettings> procs = _unitOfWork.ProcessMonitoringSettings.GetByMachine(machineId);
                if (procs != null)
                {
                    foreach (ProcessSettings proc in procs)
                    {
                        currentMonitoredProcs.Add(new SelectListItem
                        {
                            Value = proc.ProcessName,
                            Text = proc.ProcessName,
                            Selected = false
                        });
                    }
                }

                IEnumerable<EventLogSettings> logs = _unitOfWork.EventLogMonitoringSettings.GetByMachine(machineId);
                if (logs != null)
                {
                    foreach (EventLogSettings log in logs)
                    {
                        currentMonitoredLogs.Add(new SelectListItem()
                        {
                            Value = log.EventLogName,
                            Text = log.EventLogName,
                            Selected = false
                        });
                    }
                }

                IEnumerable<ServiceSettings> services = _unitOfWork.ServiceMonitoringSettings.GetByMachine(machineId);
                if (services != null)
                {
                    foreach (ServiceSettings service in services)
                    {
                        currentMonitoredServices.Add(new SelectListItem()
                        {
                            Value = service.ServiceName,
                            Text = service.ServiceName,
                            Selected = false
                        });
                    }
                }

                MachineConfigurationViewModel viewModel = new MachineConfigurationViewModel()
                {
                    MachineId = machineToUpdate.MachineID,
                    ParentEnvironmentId = machineToUpdate.ParentEnv.ToString(),
                    DisplayName = machineToUpdate.DisplayName,
                    MachineName = machineToUpdate.ComputerName,
                    IpAddress = machineToUpdate.IPV4,
                    FQDN = machineToUpdate.FQDN,
                    OperatingSystemId = machineToUpdate.OperatingSys.OperatingSysID.ToString(),
                    NumProcessors = machineToUpdate.NumProcessors,
                    TotalMemGbs = machineToUpdate.TotalMemGbs,
                    ParentEnvironmentOptions = parentEnvOps,
                    OperatingSystemOptions = operatingSysOps,
                    CurrentMonitoredProcesses = currentMonitoredProcs,
                    CurrentMonitoredEventLogs = currentMonitoredLogs,
                    CurrentMonitoredServices = currentMonitoredServices,
                    BaseAppUrl = GetBaseApplicationUrl()
                };

                PerformanceSettings perfSettings = _unitOfWork.PerformanceMonitoringSettings.Get(machineId);
                if (perfSettings != null)
                {
                    viewModel.AvgCpuUtilAlertsOn = perfSettings.AvgCpuUtilAlertsOn;
                    viewModel.AvgCpuUtilWarnValue = perfSettings.AvgCpuUtilWarnValue != null ? (int)perfSettings.AvgCpuUtilWarnValue : 0;
                    viewModel.AvgCpuUtilAlertValue = perfSettings.AvgCpuUtilAlertValue != null ? (int)perfSettings.AvgCpuUtilAlertValue : 0;
                    viewModel.HighCpuUtilAlertsOn = perfSettings.CpuHighUtilAlertsOn;
                    viewModel.HighCpuUtilWarnValue = perfSettings.CpuHighUtilWarnValue != null ? (int)perfSettings.CpuHighUtilWarnValue : 0;
                    viewModel.HighCpuUtilAlertValue = perfSettings.CpuHighUtilAlertValue != null ? (int)perfSettings.CpuHighUtilAlertValue : 0;
                    viewModel.AvgMemUtilAlertsOn = perfSettings.AvgMemUtilAlertsOn;
                    viewModel.AvgMemUtilWarnValue = perfSettings.AvgMemUtilWarnValue != null ? (int)perfSettings.AvgMemUtilWarnValue : 0;
                    viewModel.AvgMemUtilAlertValue = perfSettings.AvgMemUtilAlertValue != null ? (int)perfSettings.AvgMemUtilAlertValue : 0;
                    viewModel.HighMemUtilAlertsOn = perfSettings.MemHighUtilAlertsOn;
                    viewModel.HighMemUtilWarnValue = perfSettings.MemHighUtilWarnValue != null ? (int)perfSettings.MemHighUtilWarnValue : 0;
                    viewModel.HighMemUtilAlertValue = perfSettings.MemHighUtilAlertsValue != null ? (int)perfSettings.MemHighUtilAlertsValue : 0;
                }

                DiskSettings diskSettings = _unitOfWork.DiskMonitoringSettings.Get(machineId);
                if (diskSettings != null)
                {
                    viewModel.UsedSpaceAlertsOn = diskSettings.UsedSpaceAlertsOn;
                    viewModel.UsedSpaceWarnValue = diskSettings.UsedSpaceWarningValue;
                    viewModel.UsedSpaceAlertValue = diskSettings.UsedSpaceAlertValue;
                }

                return View(viewModel);
            }
        }
        // POST:
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpPost]
        public ActionResult MachineConfiguration(MachineConfigurationViewModel viewModel)
        {
            Machine machineToUpdate = _unitOfWork.Machines.Get(viewModel.MachineId);
            TestEnvironment parentEnvironment = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(Int32.Parse(viewModel.ParentEnvironmentId));

            // check if user has permission on this environment
            if (GetLoggedInUserId() != parentEnvironment.Creator)
            {
                return Json(new { success = false, error = ("Unauthorized!") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // if a machine with the new name already exists for the environment
                if (viewModel.DisplayName != machineToUpdate.DisplayName)
                {
                    if (_unitOfWork.Machines.CheckMachineExistsByEnvironmentAndDisplayName(Int32.Parse(viewModel.ParentEnvironmentId), viewModel.DisplayName))
                    {
                        return Json(new { success = false, error = "'" + parentEnvironment.EnvironmentName + "' already contains a machine with that name." }, JsonRequestBehavior.AllowGet);
                    }
                }

                // update machine details
                machineToUpdate.ParentEnv = Int32.Parse(viewModel.ParentEnvironmentId);
                machineToUpdate.DisplayName = viewModel.DisplayName;
                machineToUpdate.ComputerName = viewModel.MachineName;
                machineToUpdate.IPV4 = viewModel.IpAddress;
                machineToUpdate.FQDN = viewModel.FQDN;
                machineToUpdate.OS = Int32.Parse(viewModel.OperatingSystemId);
                machineToUpdate.NumProcessors = viewModel.NumProcessors;
                machineToUpdate.TotalMemGbs = viewModel.TotalMemGbs;

                // updating/maintaining performance & disk monitoring settings
                var tempPerfSetting = _unitOfWork.PerformanceMonitoringSettings.Get(viewModel.MachineId);
                if (tempPerfSetting != null)
                {
                    _unitOfWork.PerformanceMonitoringSettings.Delete(tempPerfSetting);
                }
                var tempDiskSetting = _unitOfWork.DiskMonitoringSettings.Get(viewModel.MachineId);
                if (tempDiskSetting != null)
                {
                    _unitOfWork.DiskMonitoringSettings.Delete(tempDiskSetting);
                }

                _unitOfWork.PerformanceMonitoringSettings.Add(new PerformanceSettings()
                {
                    MachineID = viewModel.MachineId,
                    AvgCpuUtilAlertsOn = viewModel.AvgCpuUtilAlertsOn,
                    AvgCpuUtilWarnValue = viewModel.AvgCpuUtilWarnValue,
                    AvgCpuUtilAlertValue = viewModel.AvgCpuUtilAlertValue,
                    CpuHighUtilAlertsOn = viewModel.HighCpuUtilAlertsOn,
                    CpuHighUtilWarnValue = viewModel.HighCpuUtilWarnValue,
                    CpuHighUtilAlertValue = viewModel.HighCpuUtilAlertValue,
                    AvgMemUtilAlertsOn = viewModel.AvgMemUtilAlertsOn,
                    AvgMemUtilWarnValue = viewModel.AvgMemUtilWarnValue,
                    AvgMemUtilAlertValue = viewModel.AvgMemUtilAlertValue,
                    MemHighUtilAlertsOn = viewModel.HighMemUtilAlertsOn,
                    MemHighUtilWarnValue = viewModel.HighMemUtilWarnValue,
                    MemHighUtilAlertsValue = viewModel.HighMemUtilAlertValue
                });
                // update disk monitoring settings
                _unitOfWork.DiskMonitoringSettings.Add(new DiskSettings()
                {
                    MachineID = viewModel.MachineId,
                    UsedSpaceAlertsOn = viewModel.UsedSpaceAlertsOn,
                    UsedSpaceWarningValue = viewModel.UsedSpaceWarnValue,
                    UsedSpaceAlertValue = viewModel.UsedSpaceAlertValue
                });

                // updating/maintaining dynamic monitoring settings (process/event-log/service)
                var procSettings = _unitOfWork.ProcessMonitoringSettings.GetByMachine(viewModel.MachineId);

                foreach (var setting in procSettings)
                {
                    if (!viewModel.UpdatedMonitoredProcesses.Contains(setting.ProcessName))
                    {
                        _unitOfWork.ProcessMonitoringSettings.Delete(setting);  // process no longer in monitored list - remove from db
                    }
                    else
                    {
                        viewModel.UpdatedMonitoredProcesses.Remove(setting.ProcessName);    // process already exists - remove from list so it's not added again
                    }
                }

                if (viewModel.UpdatedMonitoredProcesses != null)
                {
                    foreach (string procName in viewModel.UpdatedMonitoredProcesses)
                    {
                        _unitOfWork.ProcessMonitoringSettings.Add(new ProcessSettings()
                        {
                            MachineID = viewModel.MachineId,
                            ProcessName = procName
                        });
                    }
                }

                var logSettings = _unitOfWork.EventLogMonitoringSettings.GetByMachine(viewModel.MachineId);

                foreach (var setting in logSettings)
                {
                    if (!viewModel.UpdatedMonitoredEventLogs.Contains(setting.EventLogName))
                    {
                        _unitOfWork.EventLogMonitoringSettings.Delete(setting);
                    }
                    else
                    {
                        viewModel.UpdatedMonitoredEventLogs.Remove(setting.EventLogName);
                    }
                }

                if (viewModel.UpdatedMonitoredEventLogs != null)
                {
                    foreach (string eventLogName in viewModel.UpdatedMonitoredEventLogs)
                    {
                        _unitOfWork.EventLogMonitoringSettings.Add(new EventLogSettings()
                        {
                            MachineID = viewModel.MachineId,
                            EventLogName = eventLogName,
                            EventBacklogSize = 100
                        });
                    }
                }

                var serviceSettings = _unitOfWork.ServiceMonitoringSettings.GetByMachine(viewModel.MachineId);

                foreach (var setting in serviceSettings)
                {
                    if (!viewModel.UpdatedMonitoredServices.Contains(setting.ServiceName))
                    {
                        _unitOfWork.ServiceMonitoringSettings.Delete(setting);
                    }
                    else
                    {
                        viewModel.UpdatedMonitoredServices.Remove(setting.ServiceName);
                    }
                }

                if (viewModel.UpdatedMonitoredServices != null)
                {
                    foreach (string serviceName in viewModel.UpdatedMonitoredServices)
                    {
                        _unitOfWork.ServiceMonitoringSettings.Add(new ServiceSettings()
                        {
                            MachineID = viewModel.MachineId,
                            ServiceName = serviceName
                        });
                    }
                }

                _unitOfWork.Save();

                return Json(new { success = true, successmsg = ("Changes made successfully!") }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public PartialViewResult _DynamicMonitoringAlertsConfig(Guid machineId)
        {
            _DynamicMonitoringAlertsConfigViewModel viewModel = new _DynamicMonitoringAlertsConfigViewModel();

            viewModel.MachineId = machineId;

            var procSettings = _unitOfWork.ProcessMonitoringSettings.GetByMachine(machineId);
            var logSettings = _unitOfWork.EventLogMonitoringSettings.GetByMachine(machineId);
            var serviceSettings = _unitOfWork.ServiceMonitoringSettings.GetByMachine(machineId);

            foreach (var procsetting in procSettings)
            {
                viewModel.DynamicAlertSettings.Add(new SelectListItem() { Value = "proc", Text = procsetting.ProcessName });

                viewModel.ProcessAlertSettings.Add(new MonitoredProcessAlertSettings()
                {
                    ProcessName = procsetting.ProcessName,
                    WorkingSetAlertsOn = procsetting.WorkingSetAlertsOn,
                    WSWarnValue = procsetting.WSWarnValue != null ? (int)procsetting.WSWarnValue : 0,
                    WSAlertValue = procsetting.WSAlertValue != null ? (int)procsetting.WSAlertValue : 0,
                    PrivateBytesAlertsOn = procsetting.PrivateBytesAlertsOn,
                    PBWarnValue = procsetting.PBWarnValue != null ? (int)procsetting.PBWarnValue : 0,
                    PBAlertValue = procsetting.PBAlertValue != null ? (int)procsetting.PBAlertValue : 0,
                    VirtualBytesAlertsOn = procsetting.VirtualBytesAlertsOn,
                    VBWarnValue = procsetting.VBWarnValue != null ? (int)procsetting.VBWarnValue : 0,
                    VBAlertValue = procsetting.VBAlertValue != null ? (int)procsetting.VBAlertValue : 0
                });
            }

            foreach (var logsetting in logSettings)
            {
                viewModel.DynamicAlertSettings.Add(new SelectListItem() { Value = "log", Text = logsetting.EventLogName });

                viewModel.EventLogAlertSettings.Add(new MonitoredEventLogAlertSettings()
                {
                    EventLogName = logsetting.EventLogName,
                    WarningCountAlertsOn = logsetting.WarningCountAlertsOn,
                    WarningCountWarnValue = logsetting.WarningCountWarnValue != null ? (int)logsetting.WarningCountWarnValue : 0,
                    WarningCountAlertValue = logsetting.WarningCountAlertValue != null ? (int)logsetting.WarningCountAlertValue : 0,
                    ErrorCountAlertsOn = logsetting.ErrorCountAlertsOn,
                    ErrorCountWarnValue = logsetting.ErrorCountWarnValue != null ? (int)logsetting.ErrorCountWarnValue : 0,
                    ErrorCountAlertValue = logsetting.ErrorCountAlertValue != null ? (int)logsetting.ErrorCountAlertValue : 0,
                    NotFoundAlertsOn = logsetting.NotFoundAlertsOn,
                    NotFoundSevOptions = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = "Warning", Value = "0", Selected = logsetting.NotFoundSeverity == null ? false : logsetting.NotFoundSeverity == 0 ? true : false },
                        new SelectListItem() { Text = "Alert", Value = "1", Selected = logsetting.NotFoundSeverity == null ? false : logsetting.NotFoundSeverity == 1 ? true : false }
                    }
                });
            }

            foreach (var servicesetting in serviceSettings)
            {
                viewModel.DynamicAlertSettings.Add(new SelectListItem() { Value = "serv", Text = servicesetting.ServiceName });

                viewModel.ServiceAlertSettings.Add(new MonitoredServiceAlertSettings()
                {
                    ServiceName = servicesetting.ServiceName,
                    NotFoundAlertsOn = servicesetting.NotFoundAlertsOn,
                    NotFoundSevOptions = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = "Warning", Value = "0", Selected = servicesetting.NotFoundSeverity == null ? false : servicesetting.NotFoundSeverity == 0 ? true : false },
                        new SelectListItem() { Text = "Alert", Value = "1", Selected = servicesetting.NotFoundSeverity == null ? false : servicesetting.NotFoundSeverity == 1 ? true : false }
                    },
                    NotRunningAlertsOn = servicesetting.NotRunningAlertsOn,
                    NotRunningSevOptions = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = "Warning", Value = "0", Selected = servicesetting.NotRunningSeverity == null ? false : servicesetting.NotRunningSeverity == 0 ? true : false },
                        new SelectListItem() { Text = "Alert", Value = "1", Selected = servicesetting.NotRunningSeverity == null ? false : servicesetting.NotRunningSeverity == 1 ? true : false }
                    }
                });
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult DynamicMonitoringAlertsConfig(_DynamicMonitoringAlertsConfigViewModel viewModel)
        {
            // validate
            if (!ValidateDynamicProcessConfig(viewModel.ProcessAlertSettings))
                return Json(new { success = false, errormsg = ("Process monitoring values must be between 0 and 1,000,000 kbs.") }, JsonRequestBehavior.AllowGet);
            if(!ValidateDynamicEventLogConfig(viewModel.EventLogAlertSettings))
                return Json(new { success = false, errormsg = ("Event log counters must be between 0 and 1000 events.") }, JsonRequestBehavior.AllowGet);

            // update settings
            var procSettings = _unitOfWork.ProcessMonitoringSettings.GetByMachine(viewModel.MachineId);
            var logSettings = _unitOfWork.EventLogMonitoringSettings.GetByMachine(viewModel.MachineId);
            var serviceSettings = _unitOfWork.ServiceMonitoringSettings.GetByMachine(viewModel.MachineId);

            foreach (var persistedSetting in procSettings)
            {
                foreach (var updatedSetting in viewModel.ProcessAlertSettings)
                {
                    if (updatedSetting.ProcessName == persistedSetting.ProcessName)
                    {
                        persistedSetting.WorkingSetAlertsOn = updatedSetting.WorkingSetAlertsOn;
                        persistedSetting.WSWarnValue = updatedSetting.WSWarnValue;
                        persistedSetting.WSAlertValue = updatedSetting.WSAlertValue;
                        persistedSetting.PrivateBytesAlertsOn = updatedSetting.PrivateBytesAlertsOn;
                        persistedSetting.PBWarnValue = updatedSetting.PBWarnValue;
                        persistedSetting.PBAlertValue = updatedSetting.PBAlertValue;
                        persistedSetting.VirtualBytesAlertsOn = updatedSetting.VirtualBytesAlertsOn;
                        persistedSetting.VBWarnValue = updatedSetting.VBWarnValue;
                        persistedSetting.VBAlertValue = updatedSetting.VBAlertValue;
                    }
                }
            }

            foreach (var persistedSetting in logSettings)
            {
                foreach (var updatedSetting in viewModel.EventLogAlertSettings)
                {
                    if (updatedSetting.EventLogName == persistedSetting.EventLogName)
                    {
                        persistedSetting.WarningCountAlertsOn = updatedSetting.WarningCountAlertsOn;
                        persistedSetting.WarningCountWarnValue = updatedSetting.WarningCountWarnValue;
                        persistedSetting.WarningCountAlertValue = updatedSetting.WarningCountAlertValue;
                        persistedSetting.ErrorCountAlertsOn = updatedSetting.ErrorCountAlertsOn;
                        persistedSetting.ErrorCountWarnValue = updatedSetting.ErrorCountWarnValue;
                        persistedSetting.ErrorCountAlertValue = updatedSetting.ErrorCountAlertValue;
                        persistedSetting.NotFoundAlertsOn = updatedSetting.NotFoundAlertsOn;
                        persistedSetting.NotFoundSeverity = updatedSetting.NotFoundSeverity;
                    }
                }
            }

            foreach (var persistedSetting in serviceSettings)
            {
                foreach (var updatedSetting in viewModel.ServiceAlertSettings)
                {
                    if (updatedSetting.ServiceName == persistedSetting.ServiceName)
                    {
                        persistedSetting.NotFoundAlertsOn = updatedSetting.NotFoundAlertsOn;
                        persistedSetting.NotFoundSeverity = updatedSetting.NotFoundSeverity;
                        persistedSetting.NotRunningAlertsOn = updatedSetting.NotRunningAlertsOn;
                        persistedSetting.NotRunningSeverity = updatedSetting.NotRunningSeverity;
                    }
                }
            }

            _unitOfWork.Save();

            return Json(new { success = true, successmsg = ("Dynamic monitoring alerts configured!") }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateDynamicProcessConfig(List<MonitoredProcessAlertSettings> procSetList)
        {
            foreach (var procSets in procSetList)
            {
                if (procSets.WSWarnValue > 1000000 || procSets.WSWarnValue < 0)
                    return false;
                if (procSets.WSAlertValue > 1000000 || procSets.WSAlertValue < 0)
                    return false;
                if (procSets.PBWarnValue > 1000000 || procSets.PBWarnValue < 0)
                    return false;
                if (procSets.PBAlertValue > 1000000 || procSets.PBAlertValue < 0)
                    return false;
                if (procSets.VBWarnValue > 1000000 || procSets.VBWarnValue < 0)
                    return false;
                if (procSets.VBAlertValue > 1000000 || procSets.VBAlertValue < 0)
                    return false;
            }

            return true;
        }

        private bool ValidateDynamicEventLogConfig(List<MonitoredEventLogAlertSettings> logSetList)
        {
            foreach (var logSets in logSetList)
            {
                if (logSets.ErrorCountWarnValue > 1000 || logSets.ErrorCountWarnValue < 0)
                    return false;
                if (logSets.ErrorCountAlertValue > 1000 || logSets.ErrorCountAlertValue < 0)
                    return false;
                if (logSets.WarningCountWarnValue > 1000 || logSets.WarningCountWarnValue < 0)
                    return false;
                if (logSets.WarningCountAlertValue > 1000 || logSets.WarningCountAlertValue < 0)
                    return false;
            }

            return true;
        }

        // MachineCreation - page for creating new machines
        // GET:
        [CustomAuth(Roles = "Administrator, QA")]
        public ActionResult MachineCreation(int environmentId)
        {
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            var operatingSystems = _unitOfWork.OperatingSystems.GetAll();
            var environments = _unitOfWork.TestEnvironments.GetEnvironmentsByCreator(loggedInUserId);

            List<SelectListItem> parentEnvOps = new List<SelectListItem>();
            List<SelectListItem> operatingSysOps = new List<SelectListItem>();

            foreach (var environment in environments)
            {
                parentEnvOps.Add(new SelectListItem
                {
                    Value = environment.EnvironmentID.ToString(),
                    Text = environment.EnvironmentName,
                    Selected = (environment.EnvironmentID == environmentId ? true : false)
                });
            }

            foreach (var os in operatingSystems)
            {
                operatingSysOps.Add(new SelectListItem { Value = os.OperatingSysID.ToString(), Text = os.OSName });
            }

            MachineCreationViewModel viewModel = new MachineCreationViewModel()
            {
                ParentEnvironmentOptions = parentEnvOps,
                OperatingSystemOptions = operatingSysOps,
                SidebarRefreshUrl = GetBaseApplicationUrl()
            };

            return View(viewModel);
        }
        // POST:
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpPost]
        public ActionResult MachineCreation(MachineCreationViewModel viewModel)
        {
            TestEnvironment parentEnvironment = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(Int32.Parse(viewModel.ParentEnvironmentId));

            // check if user has permission on this environment
            if (GetLoggedInUserId() != parentEnvironment.Creator)
            {
                return Json(new { success = false, error = ("Unauthorized!") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // if a machine with this name already exists for the environment
                if (_unitOfWork.Machines.CheckMachineExistsByEnvironmentAndDisplayName(Int32.Parse(viewModel.ParentEnvironmentId), viewModel.DisplayName))
                {
                    return Json(new { success = false, error = "'" + parentEnvironment.EnvironmentName + "' already contains a machine with that name." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // add the new machine to the database
                    Machine newMachine = new Machine()
                    {
                        ParentEnv = Int32.Parse(viewModel.ParentEnvironmentId),
                        DisplayName = viewModel.DisplayName,
                        ComputerName = viewModel.MachineName,
                        IPV4 = viewModel.IpAddress,
                        FQDN = viewModel.FQDN,
                        OS = Int32.Parse(viewModel.OperatingSystemId),
                        NumProcessors = viewModel.NumProcessors,
                        TotalMemGbs = viewModel.TotalMemGbs
                    };

                    _unitOfWork.Machines.Add(newMachine);
                    _unitOfWork.Save();

                    return Json(new { success = true, successmsg = (viewModel.DisplayName + "' has been successfully added to '" + parentEnvironment.EnvironmentName) }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [CustomAuth(Roles = "Administrator, QA")]
        [HttpGet]
        public ActionResult MachineDeletion(Guid machineId)
        {
            var machine = _unitOfWork.Machines.Get(machineId);
            var machineCred = _unitOfWork.MonitoringAgentCredentials.Get(machineId);
            var sysInfoMon = _unitOfWork.SystemInfoMonitoring.Get(machineId);
            var DiskMonSet = _unitOfWork.DiskMonitoringSettings.Get(machineId);
            var PerfMonSet = _unitOfWork.PerformanceMonitoringSettings.Get(machineId);

            _unitOfWork.MonitoringAgentCredentials.Delete(machineCred);
            _unitOfWork.SystemInfoMonitoring.Delete(sysInfoMon);
            _unitOfWork.DiskMonitoringSettings.Delete(DiskMonSet);
            _unitOfWork.PerformanceMonitoringSettings.Delete(PerfMonSet);
            _unitOfWork.Machines.Delete(machine);
            _unitOfWork.Save();

            return Json(new { success = true, successmsg = ("Machine '" + machine.DisplayName + "' deleted!") }, JsonRequestBehavior.AllowGet);
        }

        private int GetMillisecondsToNextUpdate(int environmentId)
        {
            MonitoringSettings settings = _unitOfWork.MonitoringSettings.Get(environmentId);

            int interval = (int)settings.MonitoringUpdateInterval;
            DateTime currentTime = DateTime.UtcNow;
            DateTime scheduledTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0);
            scheduledTime = scheduledTime.AddMinutes((currentTime.Minute % interval == 0) ? currentTime.Minute + interval : ((currentTime.Minute + interval - 1) / interval) * interval);

            TimeSpan timeDifference = scheduledTime - currentTime;
            int nextUpdateDue = (int)timeDifference.TotalMilliseconds;

            return (nextUpdateDue + 60000);
        }

        // Method used in unit testing.
        public Guid GetExampleMachineGuid()
        {
            return _unitOfWork.Machines.GetMachineByEnvironmentAndDisplayName(1, "Example Machine").MachineID;
        }
    }
}