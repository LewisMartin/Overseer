using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    [Table("SystemInformationMonitoring")]
    public class SystemInfo
    {
        [Key, ForeignKey("Machine")]
        public Guid MachineID { get; set; }

        public string MachineName { get; set; }

        public string IPAddress { get; set; }

        public string OSName { get; set; }

        public string OSNameFriendly { get; set; }

        public string OSBitness { get; set; }

        public int? ProcessorCount { get; set; }

        public double? TotalMem { get; set; }

        public TimeSpan? UpTime { get; set; }

        // navigation properties
        public Machine Machine { get; set; }
    }
}