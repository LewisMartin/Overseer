using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class MachineCreationViewModel
    {
        [Required]
        public string ParentEnvironmentId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string MachineName { get; set; }

        public string IpAddress { get; set; }

        public string FQDN { get; set; }

        public string OperatingSystemId { get; set; }

        public int NumProcessors { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public float TotalMemGbs { get; set; }

        public IEnumerable<SelectListItem> ParentEnvironmentOptions { get; set; }

        public IEnumerable<SelectListItem> OperatingSystemOptions { get; set; }
    }
}