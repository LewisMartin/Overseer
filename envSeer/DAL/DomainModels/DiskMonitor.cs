using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    [Table("DiskMonitoring")]
    public class DiskMonitor
    {
        [Key]
        [Column(Order = 0)]
        public int MachineID { get; set; }
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

        // Foreign keys
        [ForeignKey("MachineID")]
        public Machine Machine { get; set; }
    }
}