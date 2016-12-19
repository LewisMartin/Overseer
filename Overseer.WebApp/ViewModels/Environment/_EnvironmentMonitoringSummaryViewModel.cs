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
        public HtmlString MachineNames { get; set; }

        public HtmlString DiskLabelsData { get; set; }

        public HtmlString DiskChartData { get; set; }
    }

    public class EnvProcessInfoViewModel
    {
        public HtmlString MachineNames { get; set; }

        public HtmlString ProcessNames { get; set; }

        public HtmlString ProcessData { get; set; }
    } 

    public class EnvEventLogInfoViewModel
    {
        public EnvEventLogInfoViewModel()
        {
            EventLogConcerns = new List<EventLogConcern>();
        }

        public HtmlString EventLogData { get; set; }

        public List<EventLogConcern> EventLogConcerns { get; set; }
    }

    public class EventLogConcern
    {
        public Guid MachineId { get; set; }

        public string MachineName { get; set; }

        public string EventLogName { get; set; }

        public int ErrorCount { get; set; }
    }

    public class EnvServiceInfoViewModel
    {
        public HtmlString ServiceData { get; set; }
    }
}