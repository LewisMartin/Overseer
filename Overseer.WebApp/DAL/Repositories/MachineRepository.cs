using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Overseer.WebApp.DAL.Repositories
{
    public class MachineRepository : Repository<Machine>, IMachineRepository
    {
        public MachineRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public Machine Get(Guid machineId)
        {
            return dbContext.Machine.FirstOrDefault(m => m.MachineID == machineId);
        }

        public bool CheckMachineExistsByEnvironmentAndDisplayName(int environmentId, string displayName)
        {
            return dbContext.Machine.Any(m => m.ParentEnv == environmentId && m.DisplayName == displayName);
        }

        public Machine GetMachineAndParent(Guid machineId)
        {
            return dbContext.Machine.Include(m => m.TestEnvironment).Include(m => m.OperatingSys).FirstOrDefault(m => m.MachineID == machineId);
        }

        public Machine GetMachineAndOwner(Guid machineId)
        {
            return dbContext.Machine.Include(m => m.TestEnvironment.UserAccount).FirstOrDefault(m => m.MachineID == machineId);
        }

        /* NOTE: This will return data from the 'manual' machine data table (used when monitoring for an environment isn't turned on).
        public Machine GetMachineseerDataManual(Guid machindId)
        {
            return dbContext.Machine.Include(m => m.TestEnvironment).Include(m => m.OperatingSys).Include(m => m.ManualData).FirstOrDefault(m => m.MachineID == machineId);
        }
        */

        public Machine GetMachineseerDataMonitored(Guid machineId)
        {
            return dbContext.Machine
                .Include(m => m.TestEnvironment)
                .Include(m => m.OperatingSys)
                .Include(m => m.SystemInformationData)
                .Include(m => m.PerformanceData)
                .Include(m => m.DiskData)
                .Include(m => m.ProcessConfig)
                .Include(m => m.ProcessData)
                .Include(m => m.EventLogConfig)
                .Include(m => m.EventLogData)
                .Include(m => m.ServiceConfig)
                .Include(m => m.ServiceData)
                .FirstOrDefault(m => m.MachineID == machineId);
        }

        public Machine GetMachineByEnvironmentAndDisplayName(int environmentId, string displayName)
        {
            return dbContext.Machine.FirstOrDefault(m => m.ParentEnv == environmentId && m.DisplayName == displayName);
        }

        public IEnumerable<Machine> DiscoverySearchQuery(string searchTerm, int? envId, int? bitness, int? cores, int? mem)
        {
            var initialMatches = dbContext.Machine.Include(m => m.OperatingSys).Where(m => m.DisplayName.Contains(searchTerm) || m.ComputerName.Contains(searchTerm)).AsQueryable();

            if (envId != null || bitness != null || cores != null || mem != null)
            {
                return initialMatches.Where(m => m.ParentEnv == (envId.HasValue ? envId.Value : m.ParentEnv)
                                            && m.OperatingSys.Bitness == (bitness != null ? bitness : m.OperatingSys.Bitness)
                                            && m.NumProcessors >= (cores != null ? cores : m.NumProcessors)
                                            && m.TotalMemGbs >= (mem != null ? (float?)mem : m.TotalMemGbs)).ToList();
            }
            else
            {
                return initialMatches.AsEnumerable();
            }
        }
    }
}