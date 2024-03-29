﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("DiskMonitoring")]
    public class DiskInfo
    {
        [Key, ForeignKey("Machine")]
        [Column(Order = 0)]
        public Guid MachineID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string DriveLetter { get; set; }

        public string VolumeLabel { get; set; }

        public string DriveType { get; set; } 

        public string DriveFormat { get; set; }

        public decimal? TotalSpace { get; set; }

        public decimal? FreeSpace { get; set; }

        public decimal? UsedSpace { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}