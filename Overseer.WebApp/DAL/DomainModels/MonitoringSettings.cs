using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.DomainModels
{
    public class MonitoringSettings
    {
        [Key, ForeignKey("TestEnvironment")]
        public int EnvironmentID { get; set; }

        [Required]
        public bool MonitoringEnabled { get; set; }

        public int? MonitoringUpdateInterval { get; set; }

        public int? MonitoringUpdateSchedule { get; set; }

        // navigation properties
        public TestEnvironment TestEnvironment { get; set; }
    }
}