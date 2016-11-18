using System;
using System.Collections.Generic;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System.Linq;

namespace Overseer.WebApp.DAL.Repositories
{
    public class ServiceSettingsRepository : Repository<ServiceSettings>, IServiceSettingsRepository
    {
        public ServiceSettingsRepository(OverseerDBContext context) : base(context) { }

        public ServiceSettings Get(Guid machineId, string serviceName)
        {
            return dbContext.ServiceMonitoringSettings.FirstOrDefault(s => s.MachineID == machineId && s.ServiceName == serviceName);
        }

        public IEnumerable<ServiceSettings> GetByMachine(Guid machineId)
        {
            return dbContext.ServiceMonitoringSettings.Where(s => s.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.ServiceMonitoringSettings.RemoveRange(GetByMachine(machineId));
        }
    }
}