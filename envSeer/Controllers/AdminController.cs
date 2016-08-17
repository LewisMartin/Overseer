using envSeer.DAL;
using envSeer.DAL.Core;
using envSeer.DAL.DomainModels;
using envSeer.Helpers.AuthHelpers;
using envSeer.Models;
using envSeer.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
{
    [CustomAuth(Roles = "Administrator")]
    public class AdminController : BaseController
    {
        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // get request for admin - site page
        [HttpGet]
        public ActionResult Site()
        {
            return View();
        }

        // get request for admin - users page
        [HttpGet]
        public ActionResult Users()
        {
            // getting the top 10 users by default
            IEnumerable<UserAccount> returnedUsers = _unitOfWork.Users.GetRange(0, 10);

            List<UserAccountData> viewModelUsers = new List<UserAccountData>();

            // create a userAccountData object for each user domain object
            foreach(var user in returnedUsers)
            {
                viewModelUsers.Add(new UserAccountData
                {
                    UserId = user.UserID,
                    UserName = user.UserName,
                    UserRole = _unitOfWork.UserRoles.Get(user.UserRoleID).RoleName,
                    FullName = (user.FirstName + user.LastName),
                    Email = user.Email,
                });
            }

            int results = 10;

            // create list for 'results per page' drop down
            var resPerPageOps = new List<SelectListItem>
            {
                new SelectListItem { Text = "10", Value = "10", Selected = (results == 10 ? true : false)},
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50"}
            };

            // create list for 'page selection' drop down
            var pageSelectionOps = new List<SelectListItem>();

            for (int i = 1; i <= ((_unitOfWork.Users.CountRows() + 10 - 1) / 10); i++)
            {
                pageSelectionOps.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == 1 ? true : false) });
            };

            // instantiate and assign values to ViewModel
            UserAdminViewModel viewModel = new UserAdminViewModel
            {
                userAccountData = viewModelUsers,
                resultsPerPageOptions = resPerPageOps,
                pageSelectOptions = pageSelectionOps,
                totalPages = ((_unitOfWork.Users.CountRows() + 10 - 1)/10), // Note: integer division always rounds down
                currentPage = 1,
            };

            return View(viewModel);
        }

        // POST method for 'Users' page
        [HttpPost]
        public ActionResult Users(int resPerPage, string searchTerm, int selectedPage)
        {
            // if the 'results per page' value has been upped we reset to the first page of results
            if ((resPerPage * (selectedPage - 1)) >= _unitOfWork.Users.CountRows())
            {
                selectedPage = 1;
            }

            // getting the top 10 users by default
            IEnumerable<UserAccount> returnedUsers = _unitOfWork.Users.GetRange((resPerPage * (selectedPage-1)), resPerPage);

            List<UserAccountData> viewModelUsers = new List<UserAccountData>();

            // create a userAccountData object for each user domain object
            foreach (var user in returnedUsers)
            {
                viewModelUsers.Add(new UserAccountData
                {
                    UserId = user.UserID,
                    UserName = user.UserName,
                    UserRole = _unitOfWork.UserRoles.Get(user.UserRoleID).RoleName,
                    FullName = (user.FirstName + user.LastName),
                    Email = user.Email,
                });
            }

            // create list for 'results per page' drop down
            
            var resPerPageOps = new List<SelectListItem>
            {
                new SelectListItem { Text = "10", Value = "10"},
                new SelectListItem { Text = "25", Value = "25"},
                new SelectListItem { Text = "50", Value = "50"}
            };

            // create list for 'page selection' drop down
            var pageSelectionOps = new List<SelectListItem>();

            for (int i = 1; i <= ((_unitOfWork.Users.CountRows() + resPerPage - 1) / resPerPage); i++)
            {
                pageSelectionOps.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            };

            // instantiate and assign values to ViewModel
            UserAdminViewModel viewModel = new UserAdminViewModel
            {
                userAccountData = viewModelUsers,
                resultsPerPageOptions = resPerPageOps,
                pageSelectOptions = pageSelectionOps,
                totalPages = ((_unitOfWork.Users.CountRows() + resPerPage - 1) / resPerPage), // Note: integer division always rounds down
                currentPage = 1,
            };

            //ModelState.Clear();

            return View(viewModel);
        }


        // Need to think about how user deletion/sorting/searching will work etc..
        [HttpPost]
        public ActionResult delUser(int Id)
        {



            return RedirectToAction("");
        }
    }
}