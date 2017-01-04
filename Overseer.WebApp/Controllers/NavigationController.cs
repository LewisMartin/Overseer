﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Overseer.WebApp.ViewModels.Navigation;
using System.Security.Claims;
using Overseer.WebApp.Helpers.AuthHelpers;

namespace Overseer.WebApp.Controllers
{
    // controller to provide the model for the side nav bar and contain method to return nav bar as a partial view
    [CustomAuth]
    public class NavigationController : BaseController
    {
        // GET: Navigation
        public ActionResult Index()
        {
            return View();
        }

        // GET: SidebarNav
        //[ChildActionOnly]   // can only be called from within a view
        public PartialViewResult _SidebarNav(string activeCtrl)
        {
            // cast user identity as 'ClaimIdentity' in order to access it's other claims
            var userClaims = User.Identity as ClaimsIdentity;

            _SidebarNavViewModel _sidebarNavViewModel = new _SidebarNavViewModel { userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value, activeController = activeCtrl };

            return PartialView(_sidebarNavViewModel);
        }

        // GET: EnvironmentNavigation (partial view user within sidebar)
        public PartialViewResult _EnvironmentNavigation()
        {
            // cast user identity as 'ClaimIdentity' in order to access it's other claims
            var userClaims = User.Identity as ClaimsIdentity;
            int loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);

            _EnvironmentNavigationViewModel viewModel = new _EnvironmentNavigationViewModel()
            {
                environmentNavDetails = new List<EnvironmentLinkViewModel>()
            };

            // 
            foreach (var environment in _unitOfWork.TestEnvironments.GetEnvironmentsAndChildMachinesByCreator(loggedInUserId).ToList())
            {
                List<MachineLinkViewModel> childMachines = new List<MachineLinkViewModel>();

                if (environment.Machines != null)
                {
                    foreach (var childMachine in environment.Machines)
                    {
                        childMachines.Add(new MachineLinkViewModel { MachineId = childMachine.MachineID, DisplayName = childMachine.DisplayName });
                    }
                }

                viewModel.environmentNavDetails.Add(new EnvironmentLinkViewModel {
                                                            EnvironmentId = environment.EnvironmentID,
                                                            EnvironmentName = environment.EnvironmentName,
                                                            ChildMachines = childMachines});
            }

            return PartialView(viewModel);
        }
    }
}