using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class _EnvironmentMonitoringSummaryViewModel
    {
        public _EnvironmentMonitoringSummaryViewModel()
        {
            EnvPerformanceInfo = new EnvPerformanceInfoViewModel();
            EnvDiskInfo = new EnvDiskInfoViewModel();
            EnvProcessInfo = new EnvProcessInfoViewModel();
            EnvEventLogInfo = new EnvEventLogInfoViewModel();
            EnvServiceInfo = new EnvServiceInfoViewModel();
        }

        public EnvPerformanceInfoViewModel EnvPerformanceInfo { get; set; }    // change this to a list in order to contain multiple performance 'readings'

        public EnvDiskInfoViewModel EnvDiskInfo { get; set; }

        public EnvProcessInfoViewModel EnvProcessInfo { get; set; }

        public EnvEventLogInfoViewModel EnvEventLogInfo { get; set; }

        public EnvServiceInfoViewModel EnvServiceInfo { get; set; }
    }

    public class EnvPerformanceInfoViewModel
    {
        public HtmlString MachineNames { get; set; }

        public HtmlString CpuChart { get; set; }
        
        public HtmlString MemChart { get; set; }
    }

    public class EnvDiskInfoViewModel
    {
        public HtmlString DiskChart { get; set; }
    }

    public class EnvProcessInfoViewModel
    {
        public HtmlString ProcessChart { get; set; }
    } 

    public class EnvEventLogInfoViewModel
    {
        public HtmlString EventLogChart { get; set; }
    }

    public class EnvServiceInfoViewModel
    {
        public HtmlString ServiceChart { get; set; }
    }
}