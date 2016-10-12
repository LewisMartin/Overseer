using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class MonitoringAgentCredential
    {
        [Key, ForeignKey("Machine")]
        public Guid MachineID { get; set; }

        [Required]
        public string MonitoringAgentSecret { get; set; }

        [Required]
        public string MonitoringAgentSecretSalt { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}