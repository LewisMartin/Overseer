using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class ProcessSettingsRepository : Repository<ProcessSettings>, IProcessSettingsRepository
    {
        public ProcessSettingsRepository(OverseerDBContext context) : base(context) { }

        public ProcessSettings Get(Guid machineId, string processName)
        {
            return dbContext.ProcessMonitoringSettings.FirstOrDefault(p => p.MachineID == machineId && p.ProcessName == processName);
        }

        public IEnumerable<ProcessSettings> GetByMachine(Guid machineId)
        {
            return dbContext.ProcessMonitoringSettings.Where(p => p.MachineID == machineId).ToList();
        }

        public void DeleteByMachine(Guid machineId)
        {
            dbContext.ProcessMonitoringSettings.RemoveRange(GetByMachine(machineId));
        }
    }
}