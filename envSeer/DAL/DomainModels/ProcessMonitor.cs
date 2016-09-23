using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    [Table("ProcessMonitoring")]
    public class ProcessMonitor
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public int MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(1)]
        public string ProcessName { get; set; }

        public int? PID { get; set; }

        public string Status { get; set; }

        public int? ThreadCount { get; set; }

        public DateTime? LastStartTime { get; set; }

        public TimeSpan? CpuTime { get; set; }

        public long? MemUsage { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? LastUpdated { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}