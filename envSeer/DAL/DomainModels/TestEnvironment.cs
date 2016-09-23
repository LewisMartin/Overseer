using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    public class TestEnvironment
    {
        [Key]
        public int EnvironmentID { get; set; }

        [Required]
        [StringLength(30)]
        [Index("IX_UniqueEnvironmentNamePerUser", 1)]
        public string EnvironmentName { get; set; }

        public int? RoadMap { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [Required]
        public bool MonitoringEnabled { get; set; }

        public int? MonitoringUpdateInterval { get; set; }

        public int? MonitoringUpdateSchedule { get; set; }

        // Foreign keys
        [ForeignKey("UserAccount")]
        [Index("IX_UniqueEnvironmentNamePerUser", 2)]
        public int Creator { get; set; }

        // navigation properties
        public UserAccount UserAccount { get; set; }

        public ICollection<Machine> Machines { get; set; }
    }
}