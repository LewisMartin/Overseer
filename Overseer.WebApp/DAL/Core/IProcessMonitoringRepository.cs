using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IProcessMonitoringRepository : IRepository<ProcessInfo>
    {
        // load specific process info record using composite key
        ProcessInfo Get(Guid machineId, string processName);

        // alows us to get all process entries for particular machine
        IEnumerable<ProcessInfo> GetByMachine(Guid machineId);

        // delete multiple rows which correspond to the monitored processes of a particular machine
        void DeleteByMachine(Guid machineId);
    }
}
