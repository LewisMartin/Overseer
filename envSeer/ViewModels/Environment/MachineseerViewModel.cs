using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.ViewModels.Environment
{
    public class MachineseerViewModel
    {
        public int MachineId { get; set; }

        public int ParentEnvironmentId { get; set; }

        public MachineDetailsViewModel MachineDetails { get; set; }
    }
}