using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class EnvironmentController : BaseController
    {
        // GET: OverSeer - the page compiling monitoring of all environments
        public ActionResult Overseer()
        {
            return View();
        }

        // GET: Environmentseer - page showing monitoring of all machines within specific environment
        public ActionResult Environmentseer(int environmentId)
        {
            EnvironmentseerViewModel viewModel = new EnvironmentseerViewModel();

            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetWithAllRelatedValues(environmentId);

            if (testEnv != null)
            {
                viewModel.environmentId = testEnv.EnvironmentID;
                viewModel.environmentDetails = new EnvironmentDetailsViewModel()
                {
                    EnvironmentName = testEnv.EnvironmentName,
                    PrivateEnvironment = testEnv.IsPrivate,
                    Status = testEnv.Status,
                    MonitoringEnabled = testEnv.MonitoringSettings.MonitoringEnabled,
                    MonitoringUpdateInterval = (testEnv.MonitoringSettings.MonitoringUpdateInterval).ToString()
                };

                if (!testEnv.Status && testEnv.DownTimeCatID != null)
                {
                    viewModel.environmentDetails.DownTimeCategory = testEnv.DownTimeCategory.Name;
                }

                return View(viewModel);
            }

            return HttpNotFound();
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
                viewModel.ParentEnvironmentId = machine.ParentEnv;
                viewModel.MonitoringEnabled = monSettings.MonitoringEnabled;

                viewModel.MachineDetails = new MachineDetailsViewModel()
                {
                    ParentEnvironmentName = machine.TestEnvironment.EnvironmentName,    // <--- eager loaded environment
                    DisplayName = machine.DisplayName,
                    MachineName = machine.ComputerName,
                    IpAddress = machine.IPV4,
                    FQDN = machine.FQDN,
                    OperatingSysName = machine.OperatingSys.OSName,                     // <--- eager loaded environment
                    OperatingSysBitness = machine.OperatingSys.Bitness,                 // <--- eager loaded environment
                    NumProcessors = machine.NumProcessors,
                    TotalMemGbs = machine.TotalMemGbs
                };

                if (monSettings.MonitoringEnabled)
                {
                    if (machine.SystemInformationData != null)
                    {
                        viewModel.MonitoringData.SystemInfo.MachineName = machine.SystemInformationData.MachineName;
                        viewModel.MonitoringData.SystemInfo.IPAddress = machine.SystemInformationData.IPAddress;
                        viewModel.MonitoringData.SystemInfo.OSNameFriendly = machine.SystemInformationData.OSNameFriendly;
                        viewModel.MonitoringData.SystemInfo.OSName = machine.SystemInformationData.OSName;
                        viewModel.MonitoringData.SystemInfo.OSBitness = machine.SystemInformationData.OSBitness;
                        viewModel.MonitoringData.SystemInfo.ProcessorCount = (machine.SystemInformationData.ProcessorCount != null ? (int)machine.SystemInformationData.ProcessorCount : 0);
                        viewModel.MonitoringData.SystemInfo.TotalMem = (machine.SystemInformationData.TotalMem != null ? (int)machine.SystemInformationData.TotalMem : 0);
                    }

                    if (machine.PerformanceData != null)
                    {
                        viewModel.MonitoringData.PerformanceInfo.AvgCpuUtil = (machine.PerformanceData.CpuUtil != null ? (float)machine.PerformanceData.CpuUtil : 101);
                        viewModel.MonitoringData.PerformanceInfo.AvgMemUtil = (machine.PerformanceData.MemUtil != null ? (float)machine.PerformanceData.MemUtil : 101);
                        viewModel.MonitoringData.PerformanceInfo.HighCpuUtilIndicator = (machine.PerformanceData.HighCpuUtilIndicator != null ? (float)machine.PerformanceData.HighCpuUtilIndicator : 0);
                        viewModel.MonitoringData.PerformanceInfo.HighMemUtilIndicator = (machine.PerformanceData.HighMemUtilIndicator != null ? (float)machine.PerformanceData.HighMemUtilIndicator : 0);
                        viewModel.MonitoringData.PerformanceInfo.TotalNumProcesses = (machine.PerformanceData.TotalProcesses != null ? (int)machine.PerformanceData.TotalProcesses : 0);
                    }

                    if (machine.DiskData != null)
                    {
                        foreach (DiskInfo disk in machine.DiskData)
                        {
                            viewModel.MonitoringData.DiskInfo.Drives.Add(new SingleDriveViewModel()
                            {
                                Name = disk.DriveLetter,
                                VolumeLabel = disk.VolumeLabel,
                                DriveType = disk.DriveType,
                                DriveFormat = disk.DriveFormat,
                                TotalSpace = (decimal)disk.TotalSpace,
                                FreeSpace = (decimal)disk.FreeSpace
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

                            viewModel.MonitoringData.ProcessInfo.MonitoredProcesses.Add(new ProcessGroupViewModel() { ProcessName = setting.ProcessName, Processes = procs });

                        }
                    }

                    if (machine.EventLogData != null)
                    {
                        foreach (EventLogInfo log in machine.EventLogData)
                        {
                            viewModel.MonitoringData.EventLogInfo.EventLogs.Add(new SingleEventLogViewModel()
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
                            viewModel.MonitoringData.ServiceInfo.Services.Add(new SingleServiceViewModel()
                            {
                                ServiceName = service.ServiceName,
                                Exists = service.Exists,
                                Status = service.Status,
                                StartupType = service.StartupType
                            });
                        }
                    }
                }

                return View(viewModel);
            }

            return HttpNotFound();
        }

        // EnvironmentConfiguration - page to change environment details & configure environment level monitoring settings
        // GET:
        public ActionResult EnvironmentConfiguration(int environmentId)
        {
            // a lot of this should be extracted into a service layer..
            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(environmentId);

            var downTimeCategories = _unitOfWork.DownTimeCategories.GetAll();

            List<SelectListItem> downTimeCategoryOptions = new List<SelectListItem>();

            foreach (DownTimeCategory downTimeCat in downTimeCategories)
            {
                downTimeCategoryOptions.Add(new SelectListItem() {
                    Value = downTimeCat.DownTimeCatID.ToString(),
                    Text = downTimeCat.Name,
                    Selected = (downTimeCat.DownTimeCatID == testEnv.DownTimeCatID ? true : false)
                });
            }

            List<SelectListItem> monitoringIntervalOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "5", Text = "5", Selected = (testEnv.MonitoringSettings.MonitoringUpdateInterval == 5 ? true : false) },
                new SelectListItem() { Value = "10", Text = "10", Selected = (testEnv.MonitoringSettings.MonitoringUpdateInterval == 10 ? true : false) },
                new SelectListItem() { Value = "15", Text = "15", Selected = (testEnv.MonitoringSettings.MonitoringUpdateInterval == 15 ? true : false) },
                new SelectListItem() { Value = "30", Text = "30", Selected = (testEnv.MonitoringSettings.MonitoringUpdateInterval == 30 ? true : false) },
                new SelectListItem() { Value = "60", Text = "60", Selected = (testEnv.MonitoringSettings.MonitoringUpdateInterval == 60 ? true : false) }
            };

            EnvironmentConfigurationViewModel viewModel = new EnvironmentConfigurationViewModel()
            {
                EnvironmentId = testEnv.EnvironmentID,
                EnvironmentName = testEnv.EnvironmentName,
                PrivateEnvironment = testEnv.IsPrivate,
                EnvironmentStatus = testEnv.Status,
                DownTimeCategoryOptions = downTimeCategoryOptions,
                MonitoringEnabled = testEnv.MonitoringSettings.MonitoringEnabled,
                MonitoringIntervalOptions = monitoringIntervalOptions,
                SidebarRefreshUrl = GetBaseApplicationUrl()
            };    

            return View(viewModel);
        }
        // POST:
        [HttpPost]
        public ActionResult EnvironmentConfiguration(EnvironmentConfigurationViewModel viewModel)
        {
            // get the environment to be updated
            TestEnvironment envToUpdate = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(viewModel.EnvironmentId);

            // duplicate environment checking - should be extracted to service layer
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (envToUpdate.EnvironmentName != viewModel.EnvironmentName)
            {
                if (_unitOfWork.TestEnvironments.CheckEnvironmentExistsByCreatorAndName(loggedInUserId, viewModel.EnvironmentName))
                {
                    return Json(new { success = false, error = "You already have an environment with that name.." }, JsonRequestBehavior.AllowGet);
                }
            }

            // update the TestEnvironment table
            envToUpdate.EnvironmentName = viewModel.EnvironmentName;
            envToUpdate.IsPrivate = viewModel.PrivateEnvironment;
            envToUpdate.Status = viewModel.EnvironmentStatus;
            envToUpdate.MonitoringSettings.MonitoringEnabled = viewModel.MonitoringEnabled;

            // if the environment status is set to 'down', we update the cown time category
            if (viewModel.EnvironmentStatus == false && viewModel.DownTimeCategory != null)
            {
                envToUpdate.DownTimeCatID = Int32.Parse(viewModel.DownTimeCategory);
            }

            // if monitoring is enabled, we update the monitoring interval
            if (viewModel.MonitoringEnabled && viewModel.MonitoringUpdateInterval != null)
            {
                envToUpdate.MonitoringSettings.MonitoringUpdateInterval = Int32.Parse(viewModel.MonitoringUpdateInterval);
            }

            _unitOfWork.Save();

            return Json(new { success = true, successmsg = ("<i>Changes made successfully!</i>") }, JsonRequestBehavior.AllowGet);
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

        // EnvironmentCreation - page for creating new environments
        // GET:
        [HttpGet]
        public ActionResult EnvironmentCreation()
        {
            var downTimeCategories = _unitOfWork.DownTimeCategories.GetAll();

            List<SelectListItem> downTimeCategoryOptions = new List<SelectListItem>();

            foreach (DownTimeCategory downTimeCat in downTimeCategories)
            {
                downTimeCategoryOptions.Add(new SelectListItem() { Value = downTimeCat.DownTimeCatID.ToString(), Text = downTimeCat.Name });
            }

            EnvironmentCreationViewModel viewModel = new EnvironmentCreationViewModel()
            {
                DownTimeCategoryOptions = downTimeCategoryOptions,
                SidebarRefreshUrl = GetBaseApplicationUrl()
            };

            return View(viewModel);
        }
        // POST:
        [HttpPost]
        public ActionResult EnvironmentCreation(EnvironmentCreationViewModel viewModel)
        {
            // duplicate environment checking - should be extracted to service layer
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (_unitOfWork.TestEnvironments.CheckEnvironmentExistsByCreatorAndName(loggedInUserId, viewModel.EnvironmentName))
            {
                return Json(new { success = false, error = "You already have an environment with that name.." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                MonitoringSettings defaultMonSettings = new MonitoringSettings()
                {
                    MonitoringEnabled = false,
                    MonitoringUpdateInterval = 15,
                };

                // add the new test environment
                TestEnvironment newTestEnv = new TestEnvironment()
                {
                    EnvironmentName = viewModel.EnvironmentName,
                    Creator = loggedInUserId,
                    IsPrivate = viewModel.PrivateEnvironment,
                    Status = viewModel.EnvironmentStatus,
                    // Note below: adding entry to monitoringsetting table via navigation property
                    MonitoringSettings = defaultMonSettings
                };

                // if the environment status is set to 'down', we set the down time category (otherwise we leave as null)
                if (viewModel.EnvironmentStatus == false && viewModel.DownTimeCategory != null)
                {
                    newTestEnv.DownTimeCatID = Int32.Parse(viewModel.DownTimeCategory);
                }

                _unitOfWork.TestEnvironments.Add(newTestEnv);
                _unitOfWork.Save();

                return Json(new { success = true, successmsg = ("<i>'" + viewModel.EnvironmentName + "' created successfully!</i>") }, JsonRequestBehavior.AllowGet);
            }
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
                parentEnvOps.Add(new SelectListItem {
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
    }
}