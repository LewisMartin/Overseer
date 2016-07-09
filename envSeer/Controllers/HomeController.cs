﻿using envSeer.DAL.DomainModels;
using envSeer.Helpers.AuthHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
{
    [CustomAuth]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}