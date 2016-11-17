using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class EnvironmentCreationViewModel
    {
        [Required]
        public string EnvironmentName { get; set; }

        [Required]
        public bool PrivateEnvironment { get; set; }

        [Required]
        public bool EnvironmentStatus { get; set; }

        public string DownTimeCategory { get; set; }

        public IEnumerable<SelectListItem> DownTimeCategoryOptions { get; set; }

        public string SidebarRefreshUrl { get; set; }
    }
}