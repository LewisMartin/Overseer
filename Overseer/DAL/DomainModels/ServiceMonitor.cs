using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("ServiceMonitoring")]
    public class ServiceMonitor
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(1)]
        public string ServiceName { get; set; }

        public string Status { get; set; }

        public string StartupType { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}