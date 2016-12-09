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

        public bool Status { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan CpuTime { get; set; }

        public int ThreadCount { get; set; }

        public long PrivateWorkingSet { get; set; }

        public long CommitSize { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}