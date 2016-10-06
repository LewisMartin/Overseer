using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class EnvironmentDetailsViewModel
    {
        [Required]
        [StringLength(30)]
        public string EnvironmentName { get; set; }

        [Required]
        public bool Status { get; set; }

        public string DownTimeCategory { get; set; }

        [Required]
        public bool PrivateEnvironment { get; set; }

        [Required]
        public bool MonitoringEnabled { get; set; }

        [Required]
        public string MonitoringUpdateInterval { get; set; }
    }
}