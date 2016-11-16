using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class EventLogMonitoringRepository : Repository<EventLogInfo>, IEventLogMonitoringRepository
    {
        public EventLogMonitoringRepository(OverseerDBContext context) : base(context) { }

        public EventLogInfo Get(Guid machineId, string eventLogName)
        {
            return dbContext.EventLogMonitoring.FirstOrDefault(l => l.MachineID == machineId && l.EventLogName == eventLogName);
        }

        public IEnumerable<EventLogInfo> GetByMachine(Guid machineId)
        {
            return dbContext.EventLogMonitoring.Where(l => l.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.EventLogMonitoring.RemoveRange(GetByMachine(machineId));
        }
    }
}