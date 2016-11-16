using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("EventLogMonitoring")]
    public class EventLogInfo
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(25)]
        public string EventLogName { get; set; }

        public bool? Exists { get; set; }

        public int? NumEventsToTrawl { get; set; }

        public int? NumInfos { get; set; }

        public int? NumWarnings { get; set; }

        public int? NumErrors { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}