using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class _MonitoringSystemInfoViewModel
    {
        public string MachineName { get; set; }

        public string IPAddress { get; set; }

        public string OSName { get; set; }

        public string OSNameFriendly { get; set; }

        public string OSBitness { get; set; }

        public int ProcessorCount { get; set; }

        public double TotalMem { get; set; }

        public TimeSpan UpTime { get; set; }
    }
}