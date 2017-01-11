using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Calendar
{
    public class CalendarViewerViewModel
    {
        public int EnvironmentId { get; set; }

        public string EnvironmentName { get; set; }

        public string ChosenMonth { get; set; }

        public string ChosenYear { get; set; }

        public IEnumerable<SelectListItem> MonthOptions { get; set; }

        public IEnumerable<SelectListItem> YearOptions { get; set; }

        public string BaseAppUrl { get; set; }
    }
}