using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Alert
{
    public class _AlertNotificationsViewModel
    {
        public string DisplayAlertCount { get; set; }

        public int AlertCount { get; set; }

        public string DisplayWarningCount { get; set; }

        public int WarningCount { get; set; }
    }
}