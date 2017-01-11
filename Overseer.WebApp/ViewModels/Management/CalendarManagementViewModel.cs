using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Management
{
    public class CalendarManagementViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int DaysEffort { get; set; }

        [Required]
        public string SelectedEnvironment { get; set; }

        public IEnumerable<SelectListItem> EnvironmentOptions { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy")]
        public DateTime EventDate { get; set; }

        public string BaseAppUrl { get; set; }
    }
}