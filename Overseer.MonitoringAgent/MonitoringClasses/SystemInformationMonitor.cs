using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class SystemInformationMonitor : IMonitorable
    {
        private Logger _Logger;

        public SystemInformationMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _Logger.Log("Snapshot successful for: System Information");
        }
    }
}
