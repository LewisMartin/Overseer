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
            // get necessary data from users and construct view model for admin - users page
            // we use a custom viewmodel to avoid passing entire users (including password/salt) to the view
            // ViewModel needs: user data, total number of pages given the 'records per page' setting

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

            // instantiate and assign values to ViewModel
            UserAdminViewModel viewModel = new UserAdminViewModel
            {
                userAccountData = viewModelUsers,
                totalPages = 10,
                currentPage = 1,
                resultsPerPage = 10,
                searchTerm = "",
                selectedPage = 1
            };

            return View(viewModel);
        }
    }
}