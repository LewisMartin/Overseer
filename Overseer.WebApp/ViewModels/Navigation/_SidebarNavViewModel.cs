using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Navigation
{
    public class _SidebarNavViewModel
    {
        // the user's id to test that we can identify user from within controller
        public int UserId { get; set; }

        public string UserRole { get; set; }

        public string ActiveController { get; set; }
    }
}