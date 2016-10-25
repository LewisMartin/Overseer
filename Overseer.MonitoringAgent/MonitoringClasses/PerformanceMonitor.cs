using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class PerformanceMonitor : IMonitorable
    {
        private Logger _Logger;



        public PerformanceMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _Logger.Log("Snapshot successful for: Performance");
        }
    }
}
