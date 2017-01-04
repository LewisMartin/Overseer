using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.Helpers.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        public static string ActiveController(this HtmlHelper helper)
        {
            return helper.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
        }
    }
}