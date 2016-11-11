using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;

namespace Overseer.WebApp.DAL.Core
{
    public interface IDiskMonitoringRepository : IRepository<DiskInfo>
    {
        // load specific disk info record using composite key
        DiskInfo Get(Guid machineId, string driveLetter);

        // alows us to get all disk entries for particular machine
        IEnumerable<DiskInfo> GetByMachine(Guid machineId);

        // delete multiple rows which correspond to the disks of a particular machine
        void DeleteByMachine(Guid machineId);
    }
}
