using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Navigation
{
    public class EnvironmentLinkViewModel
    {
        public int EnvironmentId { get; set; }

        public string EnvironmentName { get; set; }

        public List<MachineLinkViewModel> ChildMachines { get; set; }
    }
}