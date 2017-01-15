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
        public EnvironmentCreationViewModel()
        {
            DownTimeCategoryOptions = new List<SelectListItem>();
            DiscoverabilityOptions = new List<SelectListItem>();
        }

        [Required]
        public string EnvironmentName { get; set; }

        [Required]
        public bool EnvironmentStatus { get; set; }

        public string DownTimeCategory { get; set; }

        public List<SelectListItem> DownTimeCategoryOptions { get; set; }

        [Required]
        public string Discoverability { get; set; }

        public List<SelectListItem> DiscoverabilityOptions { get; set; }

        public string SidebarRefreshUrl { get; set; }
    }
}