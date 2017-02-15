using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class _DynamicMonitoringAlertsConfigViewModel
    {
        public _DynamicMonitoringAlertsConfigViewModel()
        {
            DynamicAlertSettings = new List<SelectListItem>();
            ProcessAlertSettings = new List<MonitoredProcessAlertSettings>();
            EventLogAlertSettings = new List<MonitoredEventLogAlertSettings>();
            ServiceAlertSettings = new List<MonitoredServiceAlertSettings>();
        }

        public Guid MachineId { get; set; }

        public List<SelectListItem> DynamicAlertSettings { get; set; }

        public List<MonitoredProcessAlertSettings> ProcessAlertSettings { get; set; }

        public List<MonitoredEventLogAlertSettings> EventLogAlertSettings { get; set; }

        public List<MonitoredServiceAlertSettings> ServiceAlertSettings { get; set; }
    }

    public class MonitoredProcessAlertSettings
    {
        public string ProcessName { get; set; }

        [Required]
        public bool WorkingSetAlertsOn { get; set; }

        [Range(0, 1000000)]
        public int WSWarnValue { get; set; }

        [Range(0, 1000000)]
        public int WSAlertValue { get; set; }

        [Required]
        public bool PrivateBytesAlertsOn { get; set; }

        [Range(0, 1000000)]
        public int PBWarnValue { get; set; }

        [Range(0, 1000000)]
        public int PBAlertValue { get; set; }

        [Required]
        public bool VirtualBytesAlertsOn { get; set; }

        [Range(0, 1000000)]
        public int VBWarnValue { get; set; }

        [Range(0, 1000000)]
        public int VBAlertValue { get; set; }
    }

    public class MonitoredEventLogAlertSettings
    {
        public string EventLogName { get; set; }

        [Required]
        public bool WarningCountAlertsOn { get; set; }

        [Range(0, 1000)]
        public int WarningCountWarnValue { get; set; }

        [Range(0, 1000)]
        public int WarningCountAlertValue { get; set; }

        [Required]
        public bool ErrorCountAlertsOn { get; set; }

        [Range(0, 1000)]
        public int ErrorCountWarnValue { get; set; }

        [Range(0, 1000)]
        public int ErrorCountAlertValue { get; set; }

        [Required]
        public bool NotFoundAlertsOn { get; set; }

        public int NotFoundSeverity { get; set; }

        public List<SelectListItem> NotFoundSevOptions { get; set; }
    }

    public class MonitoredServiceAlertSettings
    {
        public  string ServiceName { get; set; }

        [Required]
        public bool NotFoundAlertsOn { get; set; }

        public int NotFoundSeverity { get; set; }

        public List<SelectListItem> NotFoundSevOptions { get; set; }

        [Required]
        public bool NotRunningAlertsOn { get; set; }

        public int NotRunningSeverity { get; set; }

        public List<SelectListItem> NotRunningSevOptions { get; set; }
    }
}