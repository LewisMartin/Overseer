using Newtonsoft.Json;
using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Machine;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class MachineController : BaseController
    {
        // GET: Machine
        public ActionResult Index()
        {
            return View();
        }

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
                viewModel.TotalMem = (machine.SystemInformationData.TotalMem != null ? (int)machine.SystemInformationData.TotalMem : 0);
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
                    viewModel.PerformanceInfo.AvgCpuUtil = (latestPerfReading.CpuUtil != null ? (float)latestPerfReading.CpuUtil : 101);
                    viewModel.PerformanceInfo.AvgMemUtil = (latestPerfReading.MemUtil != null ? (float)latestPerfReading.MemUtil : 101);
                    viewModel.PerformanceInfo.HighCpuUtilIndicator = (latestPerfReading.HighCpuUtilIndicator != null ? (float)latestPerfReading.HighCpuUtilIndicator : 0);
                    viewModel.PerformanceInfo.HighMemUtilIndicator = (latestPerfReading.HighMemUtilIndicator != null ? (float)latestPerfReading.HighMemUtilIndicator : 0);
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
        [HttpGet]
        public ActionResult MachineConfiguration(Guid machineId)
        {
            Machine machineToUpdate = _unitOfWork.Machines.GetMachineAndParent(machineId);

            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

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
                SidebarRefreshUrl = GetBaseApplicationUrl()
            };

            return View(viewModel);
        }
        // POST:
        [HttpPost]
        public ActionResult MachineConfiguration(MachineConfigurationViewModel viewModel)
        {
            Machine machineToUpdate = _unitOfWork.Machines.Get(viewModel.MachineId);
            TestEnvironment parentEnvironment = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(Int32.Parse(viewModel.ParentEnvironmentId));

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

            // delete and recreate records in process, eventlog & service monitoring tables.
            _unitOfWork.ProcessMonitoringSettings.DeleteByMachine(viewModel.MachineId);
            _unitOfWork.EventLogMonitoringSettings.DeleteByMachine(viewModel.MachineId);
            _unitOfWork.ServiceMonitoringSettings.DeleteByMachine(viewModel.MachineId);

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

            return Json(new { success = true, successmsg = ("<i>Changes made successfully!</i>") }, JsonRequestBehavior.AllowGet);
        }

        // MachineCreation - page for creating new machines
        // GET:
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
        [HttpPost]
        public ActionResult MachineCreation(MachineCreationViewModel viewModel)
        {
            TestEnvironment parentEnvironment = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(Int32.Parse(viewModel.ParentEnvironmentId));

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

                return Json(new { success = true, successmsg = ("<i>'" + viewModel.DisplayName + "' has been successfully added to '" + parentEnvironment.EnvironmentName + "'</i>") }, JsonRequestBehavior.AllowGet);
            }
        }

        // Note: this needs to be mved to service layer
        // method to get base url of application
        private string GetBaseApplicationUrl()
        {
            var Req = ControllerContext.RequestContext.HttpContext.Request;

            return Req.Url.Scheme + "://" + Req.Url.Authority + Req.ApplicationPath.TrimEnd('/');
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
    }
}