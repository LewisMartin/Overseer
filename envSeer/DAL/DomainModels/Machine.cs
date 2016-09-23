using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace envSeer.DAL.DomainModels
{
    public class Machine
    {
        [Key]
        public int MachineID { get; set; }

        [ForeignKey("TestEnvironment")]
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

        [ForeignKey("OperatingSys")]
        public int OS { get; set; }

        public int NumProcessors { get; set; }

        public float TotalMemGbs { get; set; }

        // navigation properties
        public TestEnvironment TestEnvironment { get; set; }

        public OperatingSys OperatingSys { get; set; }

        public PerformanceMonitor PerformanceMonitor { get; set; }

        public ICollection<DiskMonitor> DiskMonitors { get; set; }

        public ICollection<EventLogMonitor> EventLogMonitors { get; set; }

        public ICollection<ProcessMonitor> ProcessMonitors { get; set; }

        public ICollection<ServiceMonitor> ServiceMonitors { get; set; }
    }
}