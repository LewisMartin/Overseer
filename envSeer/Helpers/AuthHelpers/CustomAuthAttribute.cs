using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.Helpers.AuthHelpers
{
    // inherit from 'AuthorizeAttribute' built into MVC - we simply want to alter the way it handles unauthorized requests
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        // override the 'HandleUnauthorizedRequest' method so we can handle permissions errors better
        protected override void HandleUnauthorizedRequest(AuthorizationContext authContext)
        {
            // if the user is simply NOT authenticated
            if (!authContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // do the norm - redirect to login page
                base.HandleUnauthorizedRequest(authContext);
            }
            // else if the user is authenticated but not within the list of authorized roles
            else if (!this.Roles.Split(',').Any(authContext.HttpContext.User.IsInRole))
            {
                // assign the home page to the ViewResult (we'll redirect unauthorized users here for now, later change this to an error page)
                authContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Home/Index.cshtml"
                };
            }
            // else, the user is authenticated so continue through the default HandleUnauth method..
            else
            {
                base.HandleUnauthorizedRequest(authContext);
            }
            
        }
    }
}