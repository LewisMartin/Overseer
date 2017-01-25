using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.OAuth;
using Overseer.WebApp.Helpers.AuthHelpers;

namespace Overseer.WebApp
{
    // for some reason this startup class is required when using OWIN - look into why this is and annotate accordingly.
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable application to allow use of cookies to store information for signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // the name of the cookie that we'll create
                AuthenticationType = "UserAuthCookie",

                // the path we'll redirect to if out cookie is not present
                LoginPath = new PathString("/UserAuth/Login")
            });
        }
    }
}