using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("ProcessMonitoring")]
    public class ProcessInfo
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PID { get; set; }

        public string ProcessName { get; set; }

        public bool? IsRunning { get; set; }

        public string Status { get; set; }

        public int? ThreadCount { get; set; }

        public DateTime? LastStartTime { get; set; }

        public TimeSpan? CpuTime { get; set; }

        public long? MemUsage { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}