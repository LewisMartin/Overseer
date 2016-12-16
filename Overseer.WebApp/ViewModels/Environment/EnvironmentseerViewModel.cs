using System;
using System.Collections.Generic;

namespace Overseer.WebApp.ViewModels.Environment
{
    // contains EnvironmentDetailsViewModel & data for building monitoring data visualisation from
    public class EnvironmentseerViewModel
    {
        public EnvironmentseerViewModel()
        {
            BasicMachineDetails = new List<BasicMachineDetailsViewModel>();
        }

        public int EnvironmentId { get; set; }

        public EnvironmentDetailsViewModel EnvironmentDetails { get; set; }

        public List<BasicMachineDetailsViewModel> BasicMachineDetails { get; set; } 

        public string BaseAppUrl { get; set; }

        public int RefreshInterval { get; set; }
    }

    public class BasicMachineDetailsViewModel
    {
        public Guid MachineId { get; set; }

        public string Name { get; set; }

        public string OS { get; set; }

        public string Bitness { get; set; }

        public int Cores { get; set; }

        public float Memory { get; set; }

        public string LatestMonitoringUpdate { get; set; }
    }
}