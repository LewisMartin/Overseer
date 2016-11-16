using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IEventLogMonitoringRepository : IRepository<EventLogInfo>
    {
        // load specific event log info record using composite key
        EventLogInfo Get(Guid machineId, string eventLogName);

        // alows us to get all event log entries for particular machine
        IEnumerable<EventLogInfo> GetByMachine(Guid machineId);

        // delete multiple rows which correspond to the monitored event logs of a particular machine
        void DeleteByMachine(Guid machineId);
    }
}
