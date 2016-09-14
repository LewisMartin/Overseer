using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    [Table("EventLogMonitoring")]
    public class EventLogMonitor
    {
        [Key]
        [Column(Order = 0)]
        public int MachineID { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(1)]
        public string LogName { get; set; }

        public int? NumEventsToTrawl { get; set; }

        public int? NumInfos { get; set; }

        public int? NumWarnings { get; set; }

        public int? NumErrors { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? LastUpdated { get; set; }

        // Foreign keys
        [ForeignKey("MachineID")]
        public Machine Machine { get; set; }
    }
}