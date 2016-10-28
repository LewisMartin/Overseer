using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ServiceMonitor : IMonitorable<ServiceInformation>
    {
        private Logger _Logger;

        private ServiceInformation _ServiceInfo;

        public ServiceMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _ServiceInfo = new ServiceInformation();

            _Logger.Log("Snapshot successful for: Services");
        }

        public ServiceInformation GetDTO()
        {
            return _ServiceInfo;
        }
    }
}
