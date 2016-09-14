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
        public string EnvironmentName { get; set; }

        [Required]
        public int Creator { get; set; }

        public int? RoadMap { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [Required]
        public bool MonitoringEnabled { get; set; }

        public int? MonitoringUpdateInterval { get; set; }

        public int? MonitoringUpdateSchedule { get; set; }

        // Foreign keys
        [ForeignKey("Creator")]
        public UserAccount UserAccount { get; set; }
    }
}