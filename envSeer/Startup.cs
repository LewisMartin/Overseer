using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace envSeer
{
    // for some reason this startup class is required when using OWIN - look into why this is and annotate accordingly.
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                // I think the path below is what unauthorized requests get redirected to when using the 'Authorize' attribute
                LoginPath = new PathString("/UserAuth/Login")
            });
        }
    }
}