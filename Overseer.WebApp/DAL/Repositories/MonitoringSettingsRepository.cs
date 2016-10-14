using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class MonitoringSettingsRepository : Repository<MonitoringSettings>, IMonitoringSettingsRepository
    {
        public MonitoringSettingsRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public IEnumerable<MonitoringSettings> GetSettingsByMonitoringEnabled(bool enabled)
        {
            return dbContext.MonitoringSettings.Where(m => m.MonitoringEnabled == enabled).ToList();
        }
    }
}