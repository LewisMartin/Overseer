using envSeer.DAL.DomainModels;
using envSeer.Helpers.AuthHelpers;
using envSeer.ViewModels.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
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

            TestEnvironment testEnv = _unitOfWork.TestEnvironments.Get(environmentId);

            if (testEnv != null)
            {
                viewModel.environmentId = testEnv.EnvironmentID;
                viewModel.environmentDetails = new EnvironmentDetailsViewModel()
                {
                    EnvironmentName = testEnv.EnvironmentName,
                    IsOnline = testEnv.IsOnline,
                    MonitoringEnabled = testEnv.MonitoringEnabled,
                    MonitoringUpdateInterval = (testEnv.MonitoringUpdateInterval).ToString()
                };

                return View(viewModel);
            }

            return HttpNotFound();
        }

        // GET: Machineseer - page showing monitoirng of specific machine
        public ActionResult Machineseer(int machineId)
        {
            MachineseerViewModel viewModel = new MachineseerViewModel();

            Machine machine = _unitOfWork.Machines.GetMachineAndParent(machineId);

            if (machine != null)
            {
                viewModel.MachineId = machine.MachineID;
                viewModel.ParentEnvironmentId = machine.ParentEnv;

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

                return View(viewModel);
            }

            return HttpNotFound();
        }

        // GET: EnvironmentConfiguration - page to change environment details & configure environment level monitoring settings
        public ActionResult EnvironmentConfiguration()
        {
            return View();
        }

        // GET: MachineConfiguration - page to change machine details & configure machine level monitoring settings
        public ActionResult MachineConfiguration()
        {
            return View();
        }

        // GET: EnvironmentCreation - page for creating new environments
        [HttpGet]
        public ActionResult EnvironmentCreation()
        {
            EnvironmentCreationViewModel viewModel = new EnvironmentCreationViewModel()
            {
                environmentDetails = new EnvironmentDetailsViewModel(),

                MonitoringIntervalOptions = new List<SelectListItem>()
                {
                    new SelectListItem() { Value = "5", Text = "5" },
                    new SelectListItem() { Value = "10", Text = "10" },
                    new SelectListItem() { Value = "15", Text = "15" },
                    new SelectListItem() { Value = "30", Text = "30" },
                    new SelectListItem() { Value = "60", Text = "60" }
                }
            };

            return View(viewModel);
        }
        // POST: EnvironmentCreation
        [HttpPost]
        public ActionResult EnvironmentCreation(EnvironmentCreationViewModel viewModel)
        {
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            // change this to a real duplicate environment check...
            if (_unitOfWork.TestEnvironments.CheckEnvironmentExistsByCreatorAndName(loggedInUserId, viewModel.environmentDetails.EnvironmentName))
            {
                return Json(new { success = false, error = "You already have an environment with that name.." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // add the new test environment
                TestEnvironment newTestEnv = new TestEnvironment()
                {
                    EnvironmentName = viewModel.environmentDetails.EnvironmentName,
                    IsOnline = viewModel.environmentDetails.IsOnline,
                    MonitoringEnabled = viewModel.environmentDetails.MonitoringEnabled,
                    MonitoringUpdateInterval = Int32.Parse(viewModel.environmentDetails.MonitoringUpdateInterval),
                    Creator = loggedInUserId
                };

                _unitOfWork.TestEnvironments.Add(newTestEnv);
                _unitOfWork.Save();

                return Json(new { success = true, successmsg = ("<i>'" + viewModel.environmentDetails.EnvironmentName + "' created successfully!</i>") }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MachineCreation - page for creating new machines
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
                OperatingSystemOptions = operatingSysOps
            };

            return View(viewModel);
        }

        // POST: MachineCreation
        [HttpPost]
        public ActionResult MachineCreation(MachineCreationViewModel viewModel)
        {
            var parentEnvironment = _unitOfWork.TestEnvironments.Get(Int32.Parse(viewModel.ParentEnvironmentId));

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
    }
}