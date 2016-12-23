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

        public string IpAddress { get; set; }

        public string FQDN { get; set; }

        public string OperatingSystemId { get; set; }

        public int NumProcessors { get; set; }

        // performance monitoring alerts
        [Required]
        public bool AvgCpuUtilAlertsOn { get; set; }

        public int AvgCpuUtilWarnValue { get; set; }

        public int AvgCpuUtilAlertValue { get; set; }

        [Required]
        public bool HighCpuUtilAlertsOn { get; set; }

        public int HighCpuUtilWarnValue { get; set; }

        public int HighCpuUtilAlertValue { get; set; }

        [Required]
        public bool AvgMemUtilAlertsOn { get; set; }

        public int AvgMemUtilWarnValue { get; set; }

        public int AvgMemUtilAlertValue { get; set; }

        [Required]
        public bool HighMemUtilAlertsOn { get; set; }

        public int HighMemUtilWarnValue { get; set; }

        public int HighMemUtilAlertValue { get; set; }

        [Required]
        public bool UsedSpaceAlertsOn { get; set; }

        public int UsedSpaceWarnValue { get; set; }

        public int UsedSpaceAlertValue { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public float TotalMemGbs { get; set; }

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