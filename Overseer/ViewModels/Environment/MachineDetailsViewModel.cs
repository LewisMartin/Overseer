using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class MachineDetailsViewModel
    {
        public string ParentEnvironmentName { get; set; }

        public string DisplayName { get; set; }

        public string MachineName { get; set; }

        public string IpAddress { get; set; }

        public string FQDN { get; set; }

        public string OperatingSysName { get; set; }

        public int OperatingSysBitness { get; set; }

        public int NumProcessors { get; set; }

        public float TotalMemGbs { get; set; }
    }
}