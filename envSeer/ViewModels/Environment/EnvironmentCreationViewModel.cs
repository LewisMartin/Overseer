using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace envSeer.ViewModels.Environment
{
    public class EnvironmentCreationViewModel
    {
        public EnvironmentDetailsViewModel environmentDetails { get; set; }

        public IEnumerable<SelectListItem> MonitoringIntervalOptions { get; set; }
    }
}