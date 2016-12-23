using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class DiskSettings
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        public bool UsedSpaceAlertsOn { get; set; }         // enable/disable alerts on used drive space metrics

        public int UsedSpaceWarningValue { get; set; }      // percentage full at which warning will be generated

        public int UsedSpaceAlertValue { get; set; }        // percentage full at which alert will be generated

        // navigation properties
        public Machine Machine { get; set; }
    }
}