using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;

namespace Overseer.WebApp.DAL.Core
{
    public interface IServiceMonitoringRepository : IRepository<ServiceInfo>
    {
        // load specific service info record using composite key
        ServiceInfo Get(Guid machineId, string serviceName);

        // alows us to get all service entries for particular machine
        IEnumerable<ServiceInfo> GetByMachine(Guid machineId);

        // delete multiple rows which correspond to the monitored services of a particular machine
        void DeleteByMachine(Guid machineId);
    }
}
