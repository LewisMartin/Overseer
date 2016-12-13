using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class MachineseerViewModel
    {
        public MachineseerViewModel() { }

        public Guid MachineId { get; set; }

        public string DisplayName { get; set; }

        public int ParentEnvironmentId { get; set; }

        public bool MonitoringEnabled { get; set; }

        public MachineDetailsViewModel MachineDetails { get; set; }

        public string BaseAppUrl { get; set; }

        public int RefreshInterval { get; set; }
    }
}