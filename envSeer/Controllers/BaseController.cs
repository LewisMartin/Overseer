using envSeer.DAL;
using envSeer.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Controllers
{
    public class BaseController : Controller
    {
        // instantiating a private unit of work for any database work carried out within this controller
        protected readonly IUnitOfWork _unitOfWork;

        // Admin Controller Constructor - simply instantiates the UnitOfWork class (need to look into dependancy injection at some point)
        public BaseController()
        {
            _unitOfWork = new UnitOfWork(new envSeerDBContext());
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