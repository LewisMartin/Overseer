using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Overview
{
    public class _OverviewEnvironmentViewModel
    {
        public _OverviewEnvironmentViewModel()
        {
            MachineIndicators = new List<MachineIndicator>();
        }

        public int EnvironmentId { get; set; }

        public string EnvironmentName { get; set; }

        public string NextScheduledMonitoringUpdate { get; set; }

        public List<MachineIndicator> MachineIndicators { get; set; }
    }

    public class MachineIndicator
    {
        public MachineIndicator(Guid machineId, string machineName)
        {
            MachineId = machineId;
            MachineName = machineName;
            PerformanceIndicator = "info";
            DiskIndicator = "info";
            ProcessIndicator = "info";
            EventLogIndicator = "info";
            ServiceIndicator = "info";
            Alerts = new List<Alert>();
        }

        public Guid MachineId { get; set; }

        public string MachineName { get; set; }

        public string PerformanceIndicator { get; set; }

        public string DiskIndicator { get; set; }

        public string ProcessIndicator { get; set; }

        public string EventLogIndicator { get; set; }

        public string ServiceIndicator { get; set; }

        public List<Alert> Alerts { get; set; }
    }

    public class Alert
    {
        public int AlertId { get; set; }
    }
}