using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class EventLogMonitor : IMonitorable
    {
        private Logger _Logger;

        public EventLogMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _Logger.Log("Snapshot successful for: Event Log");
        }
    }
}
