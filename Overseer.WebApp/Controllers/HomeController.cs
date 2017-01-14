using Overseer.WebApp.DAL.DomainModels;
using Overseer.WebApp.Helpers.AuthHelpers;
using Overseer.WebApp.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    [CustomAuth]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var user = _unitOfWork.Users.GetWithUserRole(GetLoggedInUserId());

            HomeViewModel viewModel = new HomeViewModel()
            {
                UserId = user.UserID,
                UserName = user.UserName,
                Name = user.FirstName + " " + user.LastName,
                UserRole = user.UserRole.RoleName
            };

            return View(viewModel);
        }
    }
}