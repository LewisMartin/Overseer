using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        // override 'OnActionExecuting' to provide logged in user details to viewbag for use in layout file
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var SessionUser = _unitOfWork.Users.GetWithUserRole(GetLoggedInUserId());

            if (SessionUser != null)
            {
                ViewBag.SessionUserId = SessionUser.UserID;
                ViewBag.SessionUserRole = SessionUser.UserRole.RoleName;
            }
        }

        // Note: these need to be moved to service layer
        // get the currently logged in user
        protected int GetLoggedInUserId()
        {
            // cast user identity as 'ClaimIdentity' in order to access it's other claims
            var userClaims = User.Identity as ClaimsIdentity;

            int loggedInUserId = 0;

            if (userClaims.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                loggedInUserId = loggedInUserId = Int32.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            return loggedInUserId;
        }

        // method to get base url of application
        protected string GetBaseApplicationUrl()
        {
            var Req = ControllerContext.RequestContext.HttpContext.Request;

            return Req.Url.Scheme + "://" + Req.Url.Authority + Req.ApplicationPath.TrimEnd('/');
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