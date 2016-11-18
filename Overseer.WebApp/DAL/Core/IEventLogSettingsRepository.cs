using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;

namespace Overseer.WebApp.DAL.Core
{
    public interface IEventLogSettingsRepository : IRepository<EventLogSettings>
    {
        EventLogSettings Get(Guid machineId, string eventLogName);

        IEnumerable<EventLogSettings> GetByMachine(Guid machineId);

        void DeleteByMachine(Guid machineId);
    }
}
