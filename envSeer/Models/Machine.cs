using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace envSeer.Models
{
    public class Machine
    {
        [Key]
        public int MachineID { get; set; }

        [Required]
        public int ParentEnv { get; set; }

        [Required]
        [StringLength(20)]
        public string ComputerName { get; set; }

        [StringLength(30)]
        public string FQDN { get; set; }    // strings are inherently nullable (an untouched textbox = null)

        [StringLength(20)]
        public string OS { get; set; }

        [StringLength(3)]
        public string OSBitness { get; set; }

        public int? NumProcessors { get; set; }

        public float? TotalMemGbs { get; set; }

        // foreign keys
        [ForeignKey("ParentEnv")]
        public TestEnvironment Environment { get; set; }
    }
}