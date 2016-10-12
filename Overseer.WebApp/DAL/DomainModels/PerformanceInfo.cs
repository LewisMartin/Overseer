using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("PerformanceMonitoring")]
    public class PerformanceInfo
    {
        [Key, ForeignKey("Machine")]
        public Guid MachineID { get; set; }

        public int? CpuUtil { get; set; }

        public int? TotalProcesses { get; set; }

        public int? TotalThreads { get; set; }

        public int? MemUtil { get; set; }

        public int? MemUsage { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}