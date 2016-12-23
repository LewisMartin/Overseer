using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class _DynamicMonitoringAlertsConfigViewModel
    {
        public _DynamicMonitoringAlertsConfigViewModel()
        {
            ProcessAlertSettings = new List<MonitoredProcessAlertSettings>();
            EventLogAlertSettings = new List<MonitoredEventLogAlertSettings>();
            ServiceAlertSettings = new List<MonitoredServiceAlertSettings>();
        }

        public Guid MachineId { get; set; }

        public List<MonitoredProcessAlertSettings> ProcessAlertSettings { get; set; }

        public List<MonitoredEventLogAlertSettings> EventLogAlertSettings { get; set; }

        public List<MonitoredServiceAlertSettings> ServiceAlertSettings { get; set; }
    }

    public class MonitoredProcessAlertSettings
    {
        public string ProcessName { get; set; }

        [Required]
        public bool WorkingSetAlertsOn { get; set; }

        public int WSWarnValue { get; set; }

        public int WSAlertValue { get; set; }

        [Required]
        public bool PrivateBytesAlertsOn { get; set; }

        public int PBWarnValue { get; set; }

        public int PBAlertValue { get; set; }

        [Required]
        public bool VirtualBytesAlertsOn { get; set; }

        public int VBWarnValue { get; set; }

        public int VBAlertValue { get; set; }
    }

    public class MonitoredEventLogAlertSettings
    {
        public string EventLogName { get; set; }

        [Required]
        public bool WarningCountAlertsOn { get; set; }

        public int WarningCountWarnValue { get; set; }

        public int WarningCountAlertValue { get; set; }

        [Required]
        public bool ErrorCountAlertsOn { get; set; }

        public int ErrorCountWarnValue { get; set; }

        public int ErrorCountAlertValue { get; set; }

        [Required]
        public bool NotFoundAlertsOn { get; set; }

        public int NotFoundSeverity { get; set; }
    }

    public class MonitoredServiceAlertSettings
    {
        public  string ServiceName { get; set; }

        [Required]
        public bool NotFoundAlertsOn { get; set; }

        public int NotFoundSeverity { get; set; }

        [Required]
        public bool NotRunningAlertsOn { get; set; }

        public int NotRunningSeverity { get; set; }
    }
}