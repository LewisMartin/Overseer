using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class TestEnvironment
    {
        [Key]
        public int EnvironmentID { get; set; }

        [Required]
        [StringLength(30)]
        [Index("IX_UniqueEnvironmentNamePerUser", 1)]
        public string EnvironmentName { get; set; }

        [ForeignKey("UserAccount")]
        [Index("IX_UniqueEnvironmentNamePerUser", 2)]
        public int Creator { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        public bool Status { get; set; }

        [ForeignKey("DownTimeCategory")]
        public int? DownTimeCatID { get; set; }

        // navigation properties
        public UserAccount UserAccount { get; set; }

        public DownTimeCategory DownTimeCategory { get; set; }

        public ICollection<Machine> Machines { get; set; }

        public MonitoringSettings MonitoringSettings { get; set; }
    }
}