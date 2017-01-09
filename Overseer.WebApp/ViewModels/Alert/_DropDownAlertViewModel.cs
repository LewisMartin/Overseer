using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Alert
{
    public class _DropDownAlertViewModel
    {
        public _DropDownAlertViewModel()
        {
            Alerts = new List<DropDownAlert>();
        }

        public List<DropDownAlert> Alerts { get; set; } 
    }

    public class DropDownAlert
    {
        public string CategoryName { get; set; }

        public string Source { get; set; }

        public string MachineName { get; set; }

        public Guid MachineId { get; set; }
        
        public string EnvironmentName { get; set; }
    }
}