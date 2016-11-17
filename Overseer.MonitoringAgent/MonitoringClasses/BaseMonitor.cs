using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public abstract class BaseMonitor
    {
        protected Logger _Logger;

        public BaseMonitor()
        {
            _Logger = Logger.Instance();
        }
    }
}
