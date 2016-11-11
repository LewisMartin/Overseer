using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class SystemInfoMonitoringRepository : Repository<SystemInfo>, ISystemInfoMonitoringRepository
    {
        public SystemInfoMonitoringRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public SystemInfo Get(Guid machineId)
        {
            return dbContext.SystemInfoMonitoring.FirstOrDefault(s => s.MachineID == machineId);
        }
    }
}