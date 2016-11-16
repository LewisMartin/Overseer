using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class ServiceMonitoringRepository : Repository<ServiceInfo>, IServiceMonitoringRepository
    {
        public ServiceMonitoringRepository(OverseerDBContext context) : base(context) { }

        public ServiceInfo Get(Guid machineId, string serviceName)
        {
            return dbContext.ServiceMonitoring.FirstOrDefault(s => s.MachineID == machineId && s.ServiceName == serviceName);
        }

        public IEnumerable<ServiceInfo> GetByMachine(Guid machineId)
        {
            return dbContext.ServiceMonitoring.Where(s => s.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.ServiceMonitoring.RemoveRange(GetByMachine(machineId));
        }
    }
}