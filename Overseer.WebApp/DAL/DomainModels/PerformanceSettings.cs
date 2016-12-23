using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class PerformanceSettings
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        public bool AvgCpuUtilAlertsOn { get; set; }        // enable/disable alerts on average Cpu usage metrics

        public int? AvgCpuUtilWarnValue { get; set; }        // percentage cpu util at which to generate warning

        public int? AvgCpuUtilAlertValue { get; set; }       // percentage cpu util at which to generate alert

        public bool CpuHighUtilAlertsOn { get; set; }       // enable/disable alerts on recorded high cpu util spikes

        public int? CpuHighUtilWarnValue { get; set; }

        public int? CpuHighUtilAlertValue { get; set; }

        public bool AvgMemUtilAlertsOn { get; set; }        // enable/disable alerts on average Mem usage metrics

        public int? AvgMemUtilWarnValue { get; set; }

        public int? AvgMemUtilAlertValue { get; set; }

        public bool MemHighUtilAlertsOn { get; set; }       // enable/disable alets on recorded high memory util spikes

        public int? MemHighUtilWarnValue { get; set; }

        public int? MemHighUtilAlertsValue { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}