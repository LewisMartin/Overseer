using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IPerformanceSettingsRepository : IRepository<PerformanceSettings>
    {
        PerformanceSettings Get(Guid machineId);
    }
}
