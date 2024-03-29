﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Overseer.WebApp.ViewModels.Alert
{
    public class _AlertFilterViewModel
    {
        public _AlertFilterViewModel()
        {
            MatchedAlerts = new List<AlertInfo>();
            PageOptions = new List<SelectListItem>();
        }

        public List<AlertInfo> MatchedAlerts { get; set; }

        public string AlertTypePassThru { get; set; }

        public string EnvFilterPassThru { get; set; }

        public string MachineFilterPassThru { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public string PaginationSelection { get; set; }

        public List<SelectListItem> PageOptions { get; set; }
    }

    public class AlertInfo
    {
        public int AlertId { get; set; }

        public int Severity { get; set; }

        public string MachineName { get; set; }

        public string EnvironmentName { get; set; }

        public string CategoryName { get; set; }

        public string Source { get; set; }

        public string TimeRecorded { get; set; }

        public bool Historical { get; set; }

        public bool Archived { get; set; }

        public string AlertDescription { get; set; }
    }
}