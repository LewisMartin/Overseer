using Overseer.WebApp.ViewModels.Overview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    public class OverviewController : BaseController
    {
        // GET: OverSeer - the page compiling monitoring of all environments
        [HttpGet]
        public ActionResult Overseer()
        {
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            var ownedEnvironments = _unitOfWork.TestEnvironments.GetEnvironmentsByCreator(loggedInUserId);

            OverseerViewModel viewModel = new OverseerViewModel();

            foreach (var environment in ownedEnvironments)
            {
                viewModel.EnvironmentIds.Add(environment.EnvironmentID);
            }

            viewModel.BaseAppUrl = GetBaseApplicationUrl();

            return View(viewModel);
        }

        [HttpGet]
        public PartialViewResult _OverviewEnvironment(int environmentId)
        {
            _OverviewEnvironmentViewModel viewModel = new _OverviewEnvironmentViewModel();

            var environment = _unitOfWork.TestEnvironments.GetEnvironmentAndChildMachines(environmentId);

            viewModel.EnvironmentId = environmentId;
            viewModel.EnvironmentName = environment.EnvironmentName;

            foreach (var machine in environment.Machines)
            {
                var machineIndicator = new MachineIndicator(machine.MachineID, machine.DisplayName);

                var alerts = _unitOfWork.MonitoringAlerts.GetActiveAlertsByMachine(machine.MachineID);

                if (machine.LastSnapshot == null)
                {
                    machineIndicator.PerformanceIndicator = "na";
                    machineIndicator.DiskIndicator = "na";
                    machineIndicator.ProcessIndicator = "na";
                    machineIndicator.EventLogIndicator = "na";
                    machineIndicator.ServiceIndicator = "na";
                }
                else if (alerts != null)
                {
                    foreach (var alert in alerts)
                    {
                        machineIndicator.Alerts.Add(new Alert() { AlertId = alert.AlertID });

                        //CONTINUE HERE
                        switch (alert.Category)
                        {
                            case 0: // perf
                                if (alert.Severity == 0)
                                    machineIndicator.PerformanceIndicator = "warn";
                                else
                                    machineIndicator.PerformanceIndicator = "alert";
                                break;
                            case 1: // disk
                                if (alert.Severity == 0)
                                    machineIndicator.DiskIndicator = "warn";
                                else
                                    machineIndicator.DiskIndicator = "alert";
                                break;
                            case 2: // process
                                if (alert.Severity == 0)
                                    machineIndicator.ProcessIndicator = "warn";
                                else
                                    machineIndicator.ProcessIndicator = "alert";
                                break;
                            case 3: // event log
                                if (alert.Severity == 0)
                                    machineIndicator.EventLogIndicator = "warn";
                                else
                                    machineIndicator.EventLogIndicator = "alert";
                                break;
                            case 4: // service
                                if (alert.Severity == 0)
                                    machineIndicator.ServiceIndicator = "warn";
                                else
                                    machineIndicator.ServiceIndicator = "alert";
                                break;
                        }
                    }
                }

                viewModel.MachineIndicators.Add(machineIndicator);
            }

            return PartialView(viewModel);
        }
    }
}