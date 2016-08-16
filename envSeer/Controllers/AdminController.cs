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

        // get request for admin - site page
        public ActionResult Site()
        {
            return View();
        }

        // get request for admin - users page
        public ActionResult Users()
        {
            return View();
        }
    }
}