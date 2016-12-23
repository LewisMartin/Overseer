using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("EventLogMonitoringSettings")]
    public class EventLogSettings
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string EventLogName { get; set; }

        public int EventBacklogSize { get; set; }

        // public int ClearEventsThreshold { get; set; } - future feature

        public bool WarningCountAlertsOn { get; set; }

        public int? WarningCountWarnValue { get; set; }

        public int? WarningCountAlertValue { get; set; }

        public bool ErrorCountAlertsOn { get; set; }

        public int? ErrorCountWarnValue { get; set; }

        public int? ErrorCountAlertValue { get; set; }

        public bool NotFoundAlertsOn { get; set; }

        public int? NotFoundSeverity { get; set; }              // 0 = warning, 1 = error

        // navigation properties
        public Machine Machine { get; set; }
    }
}