using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class DiskMonitoringRepository : Repository<DiskInfo>, IDiskMonitoringRepository
    {
        public DiskMonitoringRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public DiskInfo Get(Guid machineId, string driveLetter)
        {
            return dbContext.DiskMonitoring.FirstOrDefault(d => d.MachineID == machineId && d.DriveLetter == driveLetter);
        }

        public IEnumerable<DiskInfo> GetByMachine(Guid machineId)
        {
            return dbContext.DiskMonitoring.Where(d => d.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.DiskMonitoring.RemoveRange(GetByMachine(machineId));
        }
    }
}