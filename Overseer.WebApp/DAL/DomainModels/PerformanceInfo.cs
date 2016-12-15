﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("PerformanceMonitoring")]
    public class PerformanceInfo
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReadingNumber { get; set; }   // allows us to keep historical readings for more effective performance graphing

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