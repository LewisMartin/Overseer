using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IMonitoringSettingsRepository : IRepository<MonitoringSettings>
    {
        // get all environments that have monitoring enabled/disabled
        IEnumerable<MonitoringSettings> GetSettingsByMonitoringEnabled(bool enabled);
    }
}
