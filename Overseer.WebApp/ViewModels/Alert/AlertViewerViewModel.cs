using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Alert
{
    public class AlertViewerViewModel
    {
        public AlertViewerViewModel()
        {
            EnvironmentOptions = new List<SelectListItem>() { new SelectListItem() { Text = "any environment", Value = "empty", Selected = true } };
            MachineOptions = new List<SelectListItem>() { new SelectListItem() { Text = "any machine", Value = "empty", Selected = true } };
        }

        public string AlertType { get; set; }

        public string EnvironmentFilter { get; set; }

        public List<SelectListItem> EnvironmentOptions { get; set; }

        public List<SelectListItem> MachineOptions { get; set; }

        public string MachineFilter { get; set; }

        public string BaseAppUrl { get; set; }
    }
}