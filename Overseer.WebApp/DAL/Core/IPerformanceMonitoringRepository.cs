using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IPerformanceMonitoringRepository : IRepository<PerformanceInfo>
    {
        // we need to overload the generic get method here (machines table uses GUID as primary key)
        PerformanceInfo Get(Guid machineId);
    }
}
