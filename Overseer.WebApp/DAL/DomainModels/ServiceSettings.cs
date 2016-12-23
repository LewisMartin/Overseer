using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("ServiceMonitoringSettings")]
    public class ServiceSettings
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string ServiceName { get; set; }

        // public bool RequestStart { get; set; } - future feature

        public bool NotFoundAlertsOn { get; set; }

        public int? NotFoundSeverity { get; set; }

        public bool NotRunningAlertsOn { get; set; }

        public int? NotRunningSeverity { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}