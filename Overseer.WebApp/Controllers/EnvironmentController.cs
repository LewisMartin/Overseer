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
        // GET: OverSeer - the page compiling monitoring of all environments
        public ActionResult Overseer()
        {
            return View();
        }

        // GET: Environmentseer - page showing monitoring of all machines within specific environment
        public ActionResult Environmentseer(int environmentId)
        {
            EnvironmentseerViewModel viewModel = new EnvironmentseerViewModel();

            TestEnvironment testEnv = _unitOfWork.TestEnvironments.GetEnvironmentseerData(environmentId);

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

            List<string> machineNames = new List<string>();
            List<float> cpuChartData = new List<float>(), memChartData = new List<float>();

            if (env.Machines != null)
            {
                foreach (Machine machine in env.Machines)
                {
                    if ((machine.PerformanceData != null) && (machine.PerformanceData.Count > 0))    // permitting a machine has some performance readings
                    {
                        machineNames.Add(machine.DisplayName);
                        cpuChartData.Add((float)(machine.PerformanceData.ElementAt(0).CpuUtil));    // most recent cpu reading
                        memChartData.Add((float)(machine.PerformanceData.ElementAt(0).MemUtil));    // most recent mem reading
                    }
                }
            }
            viewModel.EnvPerformanceInfo.MachineNames = new System.Web.HtmlString(JsonConvert.SerializeObject(machineNames, Formatting.None));
            viewModel.EnvPerformanceInfo.CpuChart = new System.Web.HtmlString(JsonConvert.SerializeObject(cpuChartData, Formatting.None));
            viewModel.EnvPerformanceInfo.MemChart = new System.Web.HtmlString(JsonConvert.SerializeObject(memChartData, Formatting.None));

            return PartialView(viewModel);
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