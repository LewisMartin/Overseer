using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    public class Machine
    {
        [Key]
        public int MachineID { get; set; }

        [Required]
        public int ParentEnv { get; set; }

        [Required]
        [StringLength(20)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(30)]
        public string ComputerName { get; set; }

        [StringLength(15)]
        public string IPV4 { get; set; }

        [StringLength(50)]
        public string FQDN { get; set; }    // strings are inherently nullable (an untouched textbox = null)

        [Required]
        public int OS { get; set; }

        [StringLength(3)]
        public string OSBitness { get; set; }

        public int NumProcessors { get; set; }

        public float TotalMemGbs { get; set; }

        // Foreign keys
        [ForeignKey("ParentEnv")]
        public TestEnvironment TestEnvironment { get; set; }

        [ForeignKey("OS")]
        public OperatingSys OperatingSys { get; set; }
    }
}