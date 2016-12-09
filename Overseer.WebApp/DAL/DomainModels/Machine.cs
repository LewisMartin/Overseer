using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class Machine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MachineID { get; set; }

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

        [Column(TypeName = "DateTime2")]
        public DateTime? LastUpdated { get; set; }

        // navigation properties
        public TestEnvironment TestEnvironment { get; set; }

        public OperatingSys OperatingSys { get; set; }

        public MonitoringAgentCredential MonitoringAgentCredential { get; set; }

        public SystemInfo SystemInformationData { get; set; }

        public PerformanceInfo PerformanceData { get; set; }

        public ICollection<ProcessSettings> ProcessConfig { get; set; }

        public ICollection<EventLogSettings> EventLogConfig { get; set; }

        public ICollection<ServiceSettings> ServiceConfig { get; set; }

        public ICollection<DiskInfo> DiskData { get; set; }

        public ICollection<ProcessInfo> ProcessData { get; set; }

        public ICollection<EventLogInfo> EventLogData { get; set; }

        public ICollection<ServiceInfo> ServiceData { get; set; }
    }
}