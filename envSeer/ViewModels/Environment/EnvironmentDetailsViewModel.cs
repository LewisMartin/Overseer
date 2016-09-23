using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.ViewModels.Environment
{
    public class EnvironmentDetailsViewModel
    {
        [Required]
        [StringLength(30)]
        public string EnvironmentName { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [Required]
        public bool MonitoringEnabled { get; set; }

        [Required]
        public string MonitoringUpdateInterval { get; set; }
    }
}