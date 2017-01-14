using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.ViewModels.Alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    public class AlertController : BaseController
    {
        [HttpGet]
        public ActionResult AlertViewer(string initAlertType)
        {
            AlertViewerViewModel viewModel = new AlertViewerViewModel();

            switch (initAlertType)
            {
                case "all":
                    viewModel.AlertType = "all";
                    break;
                case "alerts":
                    viewModel.AlertType = "alerts";
                    break;
                case "warnings":
                    viewModel.AlertType = "warnings";
                    break;
            }

            var environments = _unitOfWork.TestEnvironments.GetEnvironmentsByCreator(GetLoggedInUserId());

            if (environments != null)
            {
                foreach (var env in environments)
                {
                    viewModel.EnvironmentOptions.Add(new SelectListItem() { Text = env.EnvironmentName, Value = env.EnvironmentID.ToString() });
                }
            }

            viewModel.BaseAppUrl = GetBaseApplicationUrl();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult RetrieveMachineOptions(string environmentId)
        {
            List<SelectListItem> machineOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "empty", Text = "any machine", Selected = true }
            };

            if (environmentId == "empty")
            {
                return Json(machineOptions, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var environment = _unitOfWork.TestEnvironments.GetEnvironmentAndChildMachines(Int32.Parse(environmentId));

                foreach (var machine in environment.Machines)
                {
                    machineOptions.Add(new SelectListItem() { Value = machine.MachineID.ToString(), Text = machine.DisplayName });
                }

                return Json(machineOptions, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AlertViewer(AlertViewerViewModel submittedFilters)
        {
            return RedirectToAction("_AlertFilter", new { alertType = submittedFilters.AlertType, envFilter = submittedFilters.EnvironmentFilter, machineFilter = submittedFilters.MachineFilter });
        }

        [HttpGet]
        public PartialViewResult _AlertFilter(string alertType, string envFilter, string machineFilter)
        {
            _AlertFilterViewModel viewModel = new _AlertFilterViewModel();

            IEnumerable<MonitoringAlert> alerts;

            if (alertType == "recent")
                alerts = _unitOfWork.MonitoringAlerts.GetMostRecentAlerts(GetLoggedInUserId(), 10, null);
            else
            {
                object[] queryParams = GetAlertFilterQueryParams(alertType, envFilter, machineFilter);
                alerts = _unitOfWork.MonitoringAlerts.AlertFilterQuery(GetLoggedInUserId(), (bool)queryParams[0], (int?)queryParams[1], (int?)queryParams[2], (Guid?)queryParams[3]);
            }

            foreach (var alert in alerts)
            {
                viewModel.MatchedAlerts.Add(new AlertInfo()
                {
                    AlertId = alert.AlertID,
                    Severity = alert.Severity,
                    CategoryName = GetAlertCategory(alert.Category),
                    Source = alert.Source,
                    Archived = alert.Archived,
                    TimeRecorded = alert.AlertCreationTime.ToString(),
                    Historical = alert.Historical,
                    MachineName = alert.Machine.DisplayName,
                    EnvironmentName = alert.Machine.TestEnvironment.EnvironmentName,
                    AlertDescription = ("Property: " + alert.TriggerName + " exceeded the " + (alert.AlertID == 1 ? "alert" : "warning") + " threshold with a value of " + alert.TriggerValue + ".")
                });
            }

            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult ArchiveAlert(int alertId)
        {
            var alert = _unitOfWork.MonitoringAlerts.Get(alertId);

            if (alert != null)
            {
                alert.Archived = true;
                _unitOfWork.Save();

                return Json(new { success = true, msg = ("<i>Monitoring alert archived.</i>") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, msg = ("<i>Error attempting to archive alert.</i>") }, JsonRequestBehavior.AllowGet);
            }
        }

        // need to accept both GET and POST to allow for post requests to cascade
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult _AlertNotifications()
        {
            int alertCount = _unitOfWork.MonitoringAlerts.GetAlertCount(GetLoggedInUserId());
            int warningCount = _unitOfWork.MonitoringAlerts.GetWarningCount(GetLoggedInUserId());

            _AlertNotificationsViewModel viewModel = new _AlertNotificationsViewModel()
            {
                DisplayAlertCount = alertCount > 999 ? "999+" : alertCount.ToString(),
                DisplayWarningCount = warningCount > 999 ? "999+" : warningCount.ToString(),
                AlertCount = alertCount,
                WarningCount = warningCount
            };

            return PartialView(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult _WarningDropDown(int totalAlerts)
        {
            var warnings = _unitOfWork.MonitoringAlerts.GetMostRecentAlerts(GetLoggedInUserId(), 5, 0);

            _DropDownAlertViewModel viewModel = new _DropDownAlertViewModel()
            {
                TotalAlerts = totalAlerts,
                DisplayedAlerts = warnings != null ? warnings.Count() : 0
            };

            foreach (var warning in warnings)
            {
                string catName = "";

                catName = GetAlertCategory(warning.Category);

                viewModel.Alerts.Add(new DropDownAlert()
                {
                    CategoryName = catName,
                    Source = warning.Source,
                    MachineName = warning.Machine.DisplayName,
                    MachineId = warning.Machine.MachineID,
                    EnvironmentName = warning.Machine.TestEnvironment.EnvironmentName
                });
            }

            return PartialView(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult _AlertDropDown(int totalAlerts)
        {
            var alerts = _unitOfWork.MonitoringAlerts.GetMostRecentAlerts(GetLoggedInUserId(), 5, 1);

            _DropDownAlertViewModel viewModel = new _DropDownAlertViewModel()
            {
                TotalAlerts = totalAlerts,
                DisplayedAlerts = alerts != null ? alerts.Count() : 0
            };

            foreach (var alert in alerts)
            {
                string catName = "";

                // 0: Perf, 1: Disk, 2: Process, 3: EventLog, 4: Service
                catName = GetAlertCategory(alert.Category);

                viewModel.Alerts.Add(new DropDownAlert()
                {
                    CategoryName = catName,
                    Source = alert.Source,
                    MachineName = alert.Machine.DisplayName,
                    MachineId = alert.Machine.MachineID,
                    EnvironmentName = alert.Machine.TestEnvironment.EnvironmentName
                });
            }

            return PartialView(viewModel);
        }

        private object[] GetAlertFilterQueryParams(string type, string env, string machine)
        {
            return new object[]
            {
                type == null ? false : type == "archived" ? true : false,                                                      // 'archived'
                type == null ? null : (type == "archived" || type == "all") ? (int?)null : type == "alerts" ? 1 : 0,            // 'severity'
                env == null ? null : env == "empty" ? (int?)null : Int32.Parse(env),                                           // 'environmentId'
                machine == null ? null : machine == "empty" ? (Guid?)null : new Guid(machine)                                  // 'machineId'
            };
        }

        private string GetAlertCategory(int catId)
        {
            switch (catId)
            {
                case 0:
                    return "Performance";
                case 1:
                    return "Disk";
                case 2:
                    return "Process";
                case 3:
                    return "Event Log";
                case 4:
                    return "Service";
                default:
                    return "Unknown";
            }
        }
    }
}