using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;

namespace Overseer.WebApp.DAL.Core
{
    public interface IServiceSettingsRepository : IRepository<ServiceSettings>
    {
        ServiceSettings Get(Guid machineId, string serviceName);

        IEnumerable<ServiceSettings> GetByMachine(Guid machineId);

        void DeleteByMachine(Guid machineId);
    }
}
