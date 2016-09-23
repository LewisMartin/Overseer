using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    [Table("PerformanceMonitoring")]
    public class PerformanceMonitor
    {
        [Key, ForeignKey("Machine")]
        public int MachineID { get; set; }

        public int? CpuUtil { get; set; }

        public int? TotalProcesses { get; set; }

        public int? TotalThreads { get; set; }

        public int? MemUtil { get; set; }

        public int? MemUsage { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? LastUpdated { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}