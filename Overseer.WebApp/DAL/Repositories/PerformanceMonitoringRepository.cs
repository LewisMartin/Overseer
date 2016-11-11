using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class PerformanceMonitoringRepository : Repository<PerformanceInfo>, IPerformanceMonitoringRepository
    {
        public PerformanceMonitoringRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public PerformanceInfo Get(Guid machineId)
        {
            return dbContext.PerformanceMonitoring.FirstOrDefault(m => m.MachineID == machineId);
        }
    }
}