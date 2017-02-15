using Newtonsoft.Json;
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
        // GET: Environmentseer - page showing monitoring of all machines within specific environment
        public ActionResult Environmentseer(int environmentId)
        {
            EnvironmentseerViewModel viewModel = new EnvironmentseerViewModel();

            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetEnvironmentseerData(environmentId);

            if (testEnv.Creator == GetLoggedInUserId())
                viewModel.EditPermission = true;

            if (testEnv != null)
            {
                viewModel.EnvironmentId = testEnv.EnvironmentID;
                viewModel.EnvironmentDetails = new EnvironmentDetailsViewModel()
                {
                    EnvironmentName = testEnv.EnvironmentName,
                    PrivateEnvironment = testEnv.IsPrivate,
                    Status = testEnv.Status,
                    MonitoringEnabled = testEnv.MonitoringSettings.MonitoringEnabled,
                    MonitoringUpdateInterval = (testEnv.MonitoringSettings.MonitoringUpdateInterval).ToString()
                };

                if (!testEnv.Status && testEnv.DownTimeCatID != null)
                {
                    viewModel.EnvironmentDetails.DownTimeCategory = testEnv.DownTimeCategory.Name;
                }

                foreach (Machine machine in testEnv.Machines)
                {
                    if (testEnv.MonitoringSettings.MonitoringEnabled && machine.SystemInformationData != null)
                    {
                        viewModel.BasicMachineDetails.Add(new BasicMachineDetailsViewModel()
                        {
                            MachineId = machine.MachineID,
                            Name = machine.DisplayName,
                            OS = machine.SystemInformationData.OSNameFriendly,
                            Bitness = machine.SystemInformationData.OSBitness,
                            Cores = (int)machine.SystemInformationData.ProcessorCount,
                            Memory = (float)machine.SystemInformationData.TotalMem,
                            LatestMonitoringUpdate = ((DateTime)machine.LastSnapshot).ToString("MM/dd/yyyy H:mm")
                        });
                    }
                    else
                    {
                        viewModel.BasicMachineDetails.Add(new BasicMachineDetailsViewModel()
                        {
                            MachineId = machine.MachineID,
                            Name = machine.DisplayName,
                            OS = machine.OperatingSys.OSName,
                            Bitness = "x" + machine.OperatingSys.Bitness.ToString(),
                            Cores = machine.NumProcessors,
                            Memory = machine.TotalMemGbs,
                            LatestMonitoringUpdate = "Never"
                        });
                    }
                }

                viewModel.BaseAppUrl = GetBaseApplicationUrl();
                viewModel.RefreshInterval = GetMillisecondsToNextUpdate(testEnv.EnvironmentID);

                return View(viewModel);
            }

            return HttpNotFound();
        }

        [HttpGet]
        public PartialViewResult _EnvironmentMonitoringSummary(int environmentId)
        {
            TestEnvironment env = _unitOfWork.TestEnvironments.GetEnvironmentMonitoringSummaryData(environmentId);

            _EnvironmentMonitoringSummaryViewModel viewModel = new _EnvironmentMonitoringSummaryViewModel();

            // data for performance graphs
            List<string> perfMachineNames = new List<string>();
            List<decimal> cpuChartData = new List<decimal>(), memChartData = new List<decimal>();

            // data for disk monitoring graphs
            List<string> diskMachineNames = new List<string>();
            List<List<string>> diskLabelData = new List<List<string>>();
            List<List<decimal>> diskChartData = new List<List<decimal>>();

            // data for process monitoring charts
            List<string> processMachineNames = new List<string>(), processNames = new List<string>();
            List<List<int>> processData = new List<List<int>>();

            // data for event log monitoring chart
            int[] eventlogData = new int[3];

            // data for service monitoring chart
            int[] serviceData = new int[3];

            // populating the above with data
            if (env.Machines != null)
            {
                foreach (Machine machine in env.Machines)
                {
                    // performance data
                    if ((machine.PerformanceData != null) && (machine.PerformanceData.Count > 0))    // permitting a machine has some performance readings
                    {
                        perfMachineNames.Add(machine.DisplayName);
                        cpuChartData.Add(Math.Round((decimal)(machine.PerformanceData.ElementAt(0).CpuUtil), 2));    // most recent cpu reading
                        memChartData.Add(Math.Round((decimal)(machine.PerformanceData.ElementAt(0).MemUtil), 2));    // most recent mem reading
                    }

                    // disk data
                    if ((machine.DiskData != null) && (machine.DiskData.Count > 0))                 // permitting the machine has some disk data
                    {
                        diskMachineNames.Add(machine.DisplayName);

                        List<string> tempDiskLabelList = new List<string>();
                        List<float> tempDiskUsageList = new List<float>();

                        int diskCount = 0;
                        foreach (DiskInfo disk in machine.DiskData)
                        {
                            if (diskChartData.Count <= diskCount) // add a list to store 1st/2nd/3rd/4th...nth disk readings for all machines
                            {
                                diskChartData.Add(new List<decimal>());
                            }

                            diskChartData[diskCount].Add(Math.Round((100 - (decimal)((disk.FreeSpace / disk.TotalSpace) * 100)), 2));

                            tempDiskLabelList.Add(disk.DriveLetter);
                            diskCount++;
                        }

                        diskLabelData.Add(tempDiskLabelList);
                    }

                    // process data
                    if ((machine.ProcessData != null) && (machine.ProcessData.Count > 0))           // permitting the machine has some monitored process data
                    {
                        processMachineNames.Add(machine.DisplayName);

                        // loop over process settings to get list of processes
                        foreach (ProcessSettings procSetting in machine.ProcessConfig)
                        {
                            if (!(processNames.Contains(procSetting.ProcessName)))
                            {
                                processNames.Add(procSetting.ProcessName);
                                processData.Add(new List<int>());
                            }
                        }

                        // loop over and add counts of processes for each
                        for (int i = 0; i < processNames.Count; i++)
                        {
                            int instanceCount = 0;
                            foreach (ProcessInfo proc in machine.ProcessData)
                            {
                                if (proc.ProcessName == processNames[i])
                                {
                                    instanceCount++;
                                }
                            }
                            processData[i].Add(instanceCount);
                        }
                    }

                    // event log data
                    if ((machine.EventLogData != null) && (machine.EventLogData.Count > 0))
                    {          
                        foreach (EventLogInfo log in machine.EventLogData)
                        {
                            eventlogData[0] += log.NumInfos;
                            eventlogData[1] += log.NumWarnings;
                            eventlogData[2] += log.NumErrors;

                            if (log.NumErrors > 0)
                            {
                                viewModel.EnvEventLogInfo.EventLogConcerns.Add(new EventLogConcern()
                                {
                                    MachineId = machine.MachineID,
                                    MachineName = machine.DisplayName,
                                    EventLogName = log.EventLogName,
                                    ErrorCount = log.NumErrors
                                });
                            }
                        }
                    }

                    // service data
                    if ((machine.ServiceData != null) && (machine.ServiceData.Count > 0))
                    {
                        foreach (ServiceInfo service in machine.ServiceData)
                        {
                            if (!service.Exists)
                            {
                                serviceData[2] += 1;
                            }
                            else
                            {
                                if (service.Status == "Running")
                                {
                                    serviceData[0] += 1;
                                }
                                else
                                {
                                    serviceData[1] += 1;
                                }
                            }
                        }
                    }
                }

                // map performance data to view model
                viewModel.EnvPerformanceInfo.MachineNames = new System.Web.HtmlString(JsonConvert.SerializeObject(perfMachineNames, Formatting.None));
                viewModel.EnvPerformanceInfo.CpuChart = new System.Web.HtmlString(JsonConvert.SerializeObject(cpuChartData, Formatting.None));
                viewModel.EnvPerformanceInfo.MemChart = new System.Web.HtmlString(JsonConvert.SerializeObject(memChartData, Formatting.None));

                // map disk data to view model
                viewModel.EnvDiskInfo.MachineNames = new System.Web.HtmlString(JsonConvert.SerializeObject(diskMachineNames, Formatting.None));
                viewModel.EnvDiskInfo.DiskLabelsData = new System.Web.HtmlString(JsonConvert.SerializeObject(diskLabelData, Formatting.None));
                viewModel.EnvDiskInfo.DiskChartData = new System.Web.HtmlString(JsonConvert.SerializeObject(diskChartData, Formatting.None));

                // map process data to view model
                viewModel.EnvProcessInfo.MachineNames = new System.Web.HtmlString(JsonConvert.SerializeObject(processMachineNames, Formatting.None));
                viewModel.EnvProcessInfo.ProcessNames = new System.Web.HtmlString(JsonConvert.SerializeObject(processNames, Formatting.None));
                viewModel.EnvProcessInfo.ProcessData = new System.Web.HtmlString(JsonConvert.SerializeObject(processData, Formatting.None));

                // map event log data to view model
                viewModel.EnvEventLogInfo.EventLogConcerns = viewModel.EnvEventLogInfo.EventLogConcerns.OrderByDescending(p => p.ErrorCount).Take(5).ToList();
                viewModel.EnvEventLogInfo.EventLogData = new System.Web.HtmlString(JsonConvert.SerializeObject(eventlogData, Formatting.None));

                // map service data to view model
                viewModel.EnvServiceInfo.ServiceData = new System.Web.HtmlString(JsonConvert.SerializeObject(serviceData, Formatting.None));
            }

            return PartialView(viewModel);
        }

        // EnvironmentConfiguration - page to change environment details & configure environment level monitoring settings
        // GET:
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpGet]
        public ActionResult EnvironmentConfiguration(int environmentId)
        {
            // a lot of this should be extracted into a service layer..
            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(environmentId);

            // check if user has permissions on this environment
            if (GetLoggedInUserId() != testEnv.Creator)
            {
                return View("~/Views/UserAuth/Unauthorized.cshtml");
            }
            else
            {
                var downTimeCategories = _unitOfWork.DownTimeCategories.GetAll();

                List<SelectListItem> downTimeCategoryOptions = new List<SelectListItem>();

                foreach (DownTimeCategory downTimeCat in downTimeCategories)
                {
                    downTimeCategoryOptions.Add(new SelectListItem()
                    {
                        Value = downTimeCat.DownTimeCatID.ToString(),
                        Text = downTimeCat.Name,
                        Selected = (downTimeCat.DownTimeCatID == testEnv.DownTimeCatID ? true : false)
                    });
                }

                List<SelectListItem> monitoringIntervalOptions = GetMonitoringIntervalOptions(testEnv.MonitoringSettings.MonitoringUpdateInterval);

                EnvironmentConfigurationViewModel viewModel = new EnvironmentConfigurationViewModel()
                {
                    EnvironmentId = testEnv.EnvironmentID,
                    EnvironmentName = testEnv.EnvironmentName,
                    DiscoverabilityOptions = new List<SelectListItem>()
                {
                    new SelectListItem() { Value = "Public", Text = "Public", Selected = (testEnv.IsPrivate == false ? true : false) },
                    new SelectListItem() { Value = "Private", Text = "Private", Selected = (testEnv.IsPrivate == true ? true : false) }
                },
                    EnvironmentStatus = testEnv.Status,
                    DownTimeCategoryOptions = downTimeCategoryOptions,
                    MonitoringEnabled = testEnv.MonitoringSettings.MonitoringEnabled,
                    MonitoringIntervalOptions = monitoringIntervalOptions,
                    BaseAppUrl = GetBaseApplicationUrl()
                };

                return View(viewModel);
            }
        }
        // POST:
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpPost]
        public ActionResult EnvironmentConfiguration(EnvironmentConfigurationViewModel viewModel)
        {
            // get the environment to be updated
            TestEnvironment envToUpdate = _unitOfWork.TestEnvironments.GetWithMonitoringSettings(viewModel.EnvironmentId);

            // check if user has permission on this environment
            if (GetLoggedInUserId() != envToUpdate.Creator)
            {
                return Json(new { success = false, successmsg = ("Unauthorized!") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // duplicate environment checking - should be extracted to service layer
                int loggedInUserId = GetLoggedInUserId();
                if (envToUpdate.EnvironmentName != viewModel.EnvironmentName)
                {
                    if (_unitOfWork.TestEnvironments.CheckEnvironmentExistsByCreatorAndName(loggedInUserId, viewModel.EnvironmentName))
                    {
                        return Json(new { success = false, error = "You already have an environment with that name!" }, JsonRequestBehavior.AllowGet);
                    }
                }

                // update the TestEnvironment table
                envToUpdate.EnvironmentName = viewModel.EnvironmentName;
                envToUpdate.IsPrivate = viewModel.Discoverability == "Public" ? false : true;
                envToUpdate.Status = viewModel.EnvironmentStatus;
                envToUpdate.MonitoringSettings.MonitoringEnabled = viewModel.MonitoringEnabled;

                // if the environment status is set to 'down', we update the cown time category
                if (viewModel.EnvironmentStatus == false && viewModel.DownTimeCategory != null)
                    envToUpdate.DownTimeCatID = Int32.Parse(viewModel.DownTimeCategory);

                // if monitoring is enabled, we update the monitoring interval
                if (viewModel.MonitoringEnabled && viewModel.MonitoringUpdateInterval != null)
                    envToUpdate.MonitoringSettings.MonitoringUpdateInterval = Int32.Parse(viewModel.MonitoringUpdateInterval);

                _unitOfWork.Save();

                return Json(new { success = true, successmsg = ("Changes made successfully!") }, JsonRequestBehavior.AllowGet);
            }
        }

        // EnvironmentCreation - page for creating new environments
        // GET:
        [CustomAuth(Roles = "Administrator, QA")]
        [HttpGet]
        public ActionResult EnvironmentCreation()
        {
            var downTimeCategories = _unitOfWork.DownTimeCategories.GetAll();

            List<SelectListItem> downTimeCategoryOptions = new List<SelectListItem>();

            EnvironmentCreationViewModel viewModel = new EnvironmentCreationViewModel()
            {
                DownTimeCategoryOptions = downTimeCategoryOptions,
                SidebarRefreshUrl = GetBaseApplicationUrl(),
                DiscoverabilityOptions = new List<SelectListItem>()
                {
                    new SelectListItem() { Value = "Public", Text = "Public" },
                    new SelectListItem() { Value = "Private", Text = "Private" }
                }
            };

            foreach (DownTimeCategory downTimeCat in downTimeCategories)
            {
                viewModel.DownTimeCategoryOptions.Add(new SelectListItem() { Value = downTimeCat.DownTimeCatID.ToString(), Text = downTimeCat.Name });
            }

            return View(viewModel);
        }
        // POST:
        [CustomAuth(Roles = "Administrator, QA")]
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
                    IsPrivate = viewModel.Discoverability == "Public" ? false : true,
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

        [CustomAuth(Roles = "Administrator, QA")]
        [HttpGet]
        public ActionResult EnvironmentDeletion(int environmentId)
        {
            var monSetts = _unitOfWork.MonitoringSettings.Get(environmentId);
            var environment = _unitOfWork.TestEnvironments.Get(environmentId);

            if (GetLoggedInUserId() != environment.Creator)
            {
                return Json(new { success = false, successmsg = ("Unauthorized!") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _unitOfWork.MonitoringSettings.Delete(monSetts);
                _unitOfWork.TestEnvironments.Delete(environment);
                _unitOfWork.Save();

                return Json(new { success = true, successmsg = ("Environment '" + environment.EnvironmentName + "' deleted!") }, JsonRequestBehavior.AllowGet);
            }
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

        private List<SelectListItem> GetMonitoringIntervalOptions(int? currentInterval)
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "5", Text = "5", Selected = (currentInterval == 5 ? true : false) },
                new SelectListItem() { Value = "10", Text = "10", Selected = (currentInterval == 10 ? true : false) },
                new SelectListItem() { Value = "15", Text = "15", Selected = (currentInterval == 15 ? true : false) },
                new SelectListItem() { Value = "30", Text = "30", Selected = (currentInterval == 30 ? true : false) },
                new SelectListItem() { Value = "60", Text = "60", Selected = (currentInterval == 60 ? true : false) }
            };
        }
    }
}