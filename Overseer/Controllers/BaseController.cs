using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Controllers
{
    public class BaseController : Controller
    {
        // instantiating a private unit of work for any database work carried out within this controller
        protected readonly IUnitOfWork _unitOfWork;

        // Admin Controller Constructor - simply instantiates the UnitOfWork class (need to look into dependancy injection at some point)
        public BaseController()
        {
            _unitOfWork = new UnitOfWork(new OverseerDBContext());
        }

        // overriding dispose method to add disposal of UnitOfWork class
        protected override void Dispose(bool disposing)
        {
            // adding the dispose of UnitOfWork, which will in turn dispose the DbContext#
            _unitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}