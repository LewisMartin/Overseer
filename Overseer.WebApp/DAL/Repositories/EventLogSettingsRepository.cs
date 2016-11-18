using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class EventLogSettingsRepository : Repository<EventLogSettings>, IEventLogSettingsRepository
    {
        public EventLogSettingsRepository(OverseerDBContext context) : base(context) { }

        public EventLogSettings Get(Guid machineId, string eventLogName)
        {
            return dbContext.EventLogMonitoringSettings.FirstOrDefault(l => l.MachineID == machineId && l.EventLogName == eventLogName);
        }

        public IEnumerable<EventLogSettings> GetByMachine(Guid machineId)
        {
            return dbContext.EventLogMonitoringSettings.Where(l => l.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.EventLogMonitoringSettings.RemoveRange(GetByMachine(machineId));
        }
    }
}