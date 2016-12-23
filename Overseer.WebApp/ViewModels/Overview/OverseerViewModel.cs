using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Overview
{
    public class OverseerViewModel
    {
        public OverseerViewModel()
        {
            EnvironmentIds = new List<int>();
        }

        public List<int> EnvironmentIds { get; set; }

        public string BaseAppUrl { get; set; }
    }
}