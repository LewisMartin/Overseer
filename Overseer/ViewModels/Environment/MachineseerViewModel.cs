using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class MachineseerViewModel
    {
        public Guid MachineId { get; set; }

        public int ParentEnvironmentId { get; set; }

        public MachineDetailsViewModel MachineDetails { get; set; }
    }
}