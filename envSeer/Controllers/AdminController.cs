using envSeer.Helpers.AuthHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
{
    [CustomAuth(Roles = "Administrator")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}