using System;
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
        [StringLength(1)]
        public string DriveLetter { get; set; }

        public string DriveType { get; set; } 

        public string DriveFormat { get; set; }

        public string VolumeLabel { get; set; }

        [Column(TypeName = "bigint")]
        public long? TotalSize { get; set; }

        [Column(TypeName = "bigint")]
        public long? FreeSpace { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? LastUpdated { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}