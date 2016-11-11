using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface ISystemInfoMonitoringRepository : IRepository<SystemInfo>
    {
        // we need to overload the generic get method here (machines table uses GUID as primary key)
        SystemInfo Get(Guid machineId);
    }
}
