using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ServiceMonitor : DynamicMonitor, IMonitorable<ServiceInformation>
    {
        private ServiceInformation _ServiceInfo;

        public ServiceMonitor() { }

        public void Snapshot()
        {
            _ServiceInfo = new ServiceInformation();

            if (MonitoredEntities.Count > 0)
            {
                foreach (string serviceName in MonitoredEntities)
                {
                    ServiceController service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);

                    if (service != null)
                    {
                        _ServiceInfo.Services.Add(new SingleService() { Name = serviceName, Exists = true, Status = service.Status.ToString(), StartUpType = GetServiceStartupViaWmi(serviceName)});
                    }
                    else
                    {
                        _ServiceInfo.Services.Add(new SingleService() { Name = serviceName, Exists = false });
                    }
                }
            }

            _Logger.Log("Snapshot successful for: Services");
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("SERVICE INFO: <");

            foreach (SingleService service in _ServiceInfo.Services)
            {
                SnapshotData += String.Format(" {0}: [Exists: {1}, Status: {2}, Startup type: {3}]",
                    service.Name, service.Exists, service.Status, service.StartUpType);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }

        public ServiceInformation GetDTO()
        {
            return _ServiceInfo;
        }

        private string GetServiceStartupViaWmi(string serviceName)
        {
            string query = "SELECT * FROM Win32_Service WHERE Name = '" + serviceName + "'";
            ManagementObjectSearcher wmiSearch = new ManagementObjectSearcher(query);

            try
            {
                if (wmiSearch != null)
                {
                    ManagementObjectCollection matchedSrvcs = wmiSearch.Get();

                    foreach (ManagementObject matchedSrvc in matchedSrvcs)
                    {
                        return matchedSrvc.GetPropertyValue("StartMode").ToString();
                    }
                }
                else
                {
                    return "Unid";
                }
            }
            catch (Exception wmiEx)
            {
                _Logger.Log(wmiEx.Message);
                return "Unid";
            }

            return "Unid";
        }
    }
}
