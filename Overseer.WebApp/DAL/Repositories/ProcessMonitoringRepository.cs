using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.DAL.Repositories
{
    public class ProcessMonitoringRepository : Repository<ProcessInfo>, IProcessMonitoringRepository
    {
        public ProcessMonitoringRepository(OverseerDBContext context) : base(context) { }

        public ProcessInfo Get(Guid machineId, string processName)
        {
            return dbContext.ProcessMonitoring.FirstOrDefault(p => p.MachineID == machineId && p.ProcessName == processName);
        }

        public IEnumerable<ProcessInfo> GetByMachine(Guid machineId)
        {
            return dbContext.ProcessMonitoring.Where(p => p.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.ProcessMonitoring.RemoveRange(GetByMachine(machineId));
        }
    }
}