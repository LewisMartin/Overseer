using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class PerformanceSettingsRepository : Repository<PerformanceSettings>, IPerformanceSettingsRepository
    {
        public PerformanceSettingsRepository(OverseerDBContext context) : base(context) {  }

        public PerformanceSettings Get(Guid machineId)
        {
            return dbContext.PerformanceMonitoringSettings.FirstOrDefault(p => p.MachineID == machineId);
        }
    }
}