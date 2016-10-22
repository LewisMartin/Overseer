using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class DiskMonitor : IMonitorable
    {
        private Logger _Logger;

        public DiskMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _Logger.Log("Snapshot successful for: Disk");
        }
    }
}
