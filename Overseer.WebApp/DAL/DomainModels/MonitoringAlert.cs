using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("MonitoringAlerts")]
    public class MonitoringAlert
    {
        [Key]
        public int AlertID { get; set; }

        [ForeignKey("Machine")]
        public Guid MachineId { get; set; }

        public int Category { get; set; }                   // 0: Perf, 1: Disk, 2: Process, 3: EventLog, 4: Service

        public string Source { get; set; }

        public int Severity { get; set; }                   // 0: Warning, 1: Alert

        public string TriggerName { get; set; }             // name of the monitored value that triggered the alert

        public string TriggerValue { get; set; }            // value of the alert trigger

        [Column(TypeName = "DateTime2")]
        public DateTime AlertCreationTime { get; set; }

        public bool Historical { get; set; }                // whether new monitoring data has come in since alert creation

        // navigation properties
        public Machine Machine { get; set; }
    }
}