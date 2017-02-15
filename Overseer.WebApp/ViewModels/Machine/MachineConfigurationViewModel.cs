using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class MachineConfigurationViewModel
    {
        [Required]
        public Guid MachineId { get; set; }

        [Required]
        public string ParentEnvironmentId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string MachineName { get; set; }

        [RegularExpression(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", ErrorMessage = "IP Address must be valid.")]
        public string IpAddress { get; set; }

        public string FQDN { get; set; }

        public string OperatingSystemId { get; set; }

        [Range(1, 100)]
        public int NumProcessors { get; set; }

        [Range(1, 250)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public float TotalMemGbs { get; set; }

        // performance monitoring alerts
        [Required]
        public bool AvgCpuUtilAlertsOn { get; set; }

        [Range(0, 100)]
        public int AvgCpuUtilWarnValue { get; set; }

        [Range(0, 100)]
        public int AvgCpuUtilAlertValue { get; set; }

        [Required]
        public bool HighCpuUtilAlertsOn { get; set; }

        [Range(0, 100)]
        public int HighCpuUtilWarnValue { get; set; }

        [Range(0, 100)]
        public int HighCpuUtilAlertValue { get; set; }

        [Required]
        public bool AvgMemUtilAlertsOn { get; set; }

        [Range(0, 100)]
        public int AvgMemUtilWarnValue { get; set; }

        [Range(0, 100)]
        public int AvgMemUtilAlertValue { get; set; }

        [Required]
        public bool HighMemUtilAlertsOn { get; set; }

        [Range(0, 100)]
        public int HighMemUtilWarnValue { get; set; }

        [Range(0, 100)]
        public int HighMemUtilAlertValue { get; set; }

        [Required]
        public bool UsedSpaceAlertsOn { get; set; }

        [Range(0, 100)]
        public int UsedSpaceWarnValue { get; set; }

        [Range(0, 100)]
        public int UsedSpaceAlertValue { get; set; }


        public IEnumerable<SelectListItem> ParentEnvironmentOptions { get; set; }

        public IEnumerable<SelectListItem> OperatingSystemOptions { get; set; }

        public IEnumerable<SelectListItem> CurrentMonitoredProcesses { get; set; }
        public IEnumerable<ProcessAlertSetting> MonitoredProcessAlertSettings { get; set; }
        public List<string> UpdatedMonitoredProcesses { get; set; }

        public IEnumerable<SelectListItem> CurrentMonitoredEventLogs { get; set; }
        public List<string> UpdatedMonitoredEventLogs { get; set; }

        public IEnumerable<SelectListItem> CurrentMonitoredServices { get; set; }
        public List<string> UpdatedMonitoredServices { get; set; }

        public string BaseAppUrl { get; set; }
    }

    public class ProcessAlertSetting
    {
        public bool WorkingSetAlertsOn { get; set; }
    } 
}