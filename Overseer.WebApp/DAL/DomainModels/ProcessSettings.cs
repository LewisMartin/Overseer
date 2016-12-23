using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("ProcessMonitoringSettings")]
    public class ProcessSettings
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string ProcessName { get; set; }

        // public bool KillRougeProcess { get; set; }

        public bool WorkingSetAlertsOn { get; set; }        // enable/disable alert generation on working set values of process instances

        public int? WSWarnValue { get; set; }

        public int? WSAlertValue { get; set; }

        public bool PrivateBytesAlertsOn { get; set; }      // enable/disable alert generation on private bytes values of process instances

        public int? PBWarnValue { get; set; }

        public int? PBAlertValue { get; set; }

        public bool VirtualBytesAlertsOn { get; set; }      // enable/disable alert generation on virtual bytes values of process instances

        public int? VBWarnValue { get; set; }

        public int? VBAlertValue { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}