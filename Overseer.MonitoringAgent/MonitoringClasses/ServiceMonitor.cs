using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ServiceMonitor : DynamicMonitor, IMonitorable<ServiceInformation>
    {
        private ServiceInformation _ServiceInfo;

        public ServiceMonitor() { }

        public void Snapshot()
        {
            _ServiceInfo = new ServiceInformation();

            _Logger.Log("Snapshot successful for: Services");
        }

        public ServiceInformation GetDTO()
        {
            return _ServiceInfo;
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("SERVICE INFO: <");

            foreach (string service in MonitoredEntities)
            {
                SnapshotData += String.Format(" {0}: [Exists: , Status: , Startup type: ]",
                    service);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }
    }
}
