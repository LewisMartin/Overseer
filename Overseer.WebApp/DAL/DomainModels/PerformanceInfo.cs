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

        public float? CpuUtil { get; set; }

        public float? HighCpuUtilIndicator { get; set; }

        public float? TotalProcesses { get; set; }

        //public float? TotalThreads { get; set; }

        public float? MemUtil { get; set; }

        public float? HighMemUtilIndicator { get; set; }

        //public float? MemUsage { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}