using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace envSeer.Models
{
    public class TestEnvironment
    {
        [Key]
        public int EnvironmentID { get; set; }

        [Required]
        [StringLength(30)]
        public string EnvironmentName { get; set; }

        [Required]
        public int Owner { get; set; }

        public int? RoadMap { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        // foreign keys
        [ForeignKey("Owner")]
        public UserAccount UserAccount { get; set; }
    }
}