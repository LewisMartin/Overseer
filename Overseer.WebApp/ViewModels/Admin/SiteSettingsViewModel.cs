using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Admin
{
    public class SiteSettingsViewModel
    {
        [Required(ErrorMessage = "Environment Limit required.")]
        [Range(1, 10)]
        public int EnvironmentLimit { get; set; }

        [Required(ErrorMessage = "Machine Limit required.")]
        [Range(1, 20)]
        public int MachineLimit { get; set; }

        [Required(ErrorMessage = "Monitoring setting required.")]
        public bool AllowMonitoring { get; set; }

        [Required(ErrorMessage = "User Role setting required.")]
        public bool AllowUserRoleChange { get; set; }

        [Required(ErrorMessage = "Username setting required.")]
        public bool AllowUsernameChange { get; set; }
    }
}