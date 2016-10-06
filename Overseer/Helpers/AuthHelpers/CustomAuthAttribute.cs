using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Helpers.AuthHelpers
{
    // inherit from 'AuthorizeAttribute' built into MVC - we simply want to alter the way it handles unauthorized requests
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        // override the 'HandleUnauthorizedRequest' method so we can handle permissions errors better
        protected override void HandleUnauthorizedRequest(AuthorizationContext authContext)
        {
            // if user is authenticated & does not have the role necessary to authorize them access to the requested content:
            if (authContext.HttpContext.User.Identity.IsAuthenticated && !this.Roles.Split(',').Any(authContext.HttpContext.User.IsInRole))
            {
                // assign the home page to the ViewResult (we'll redirect unauthorized users here for now, later change this to an error page)
                authContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Home/Index.cshtml"
                };
            }
            // else use the base method for handling simple authentication issues
            else
            {
                base.HandleUnauthorizedRequest(authContext);
            }
            
        }
    }
}