using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class DiskSettingsRepository : Repository<DiskSettings>, IDiskSettingsRepository
    {
        public DiskSettingsRepository(OverseerDBContext context) : base(context) { }

        public DiskSettings Get(Guid machineId)
        {
            return dbContext.DiskMonitoringSettings.FirstOrDefault(d => d.MachineID == machineId);
        }
    }
}