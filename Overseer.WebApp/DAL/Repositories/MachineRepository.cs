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

        public Machine GetMachineByEnvironmentAndDisplayName(int environmentId, string displayName)
        {
            return dbContext.Machine.FirstOrDefault(m => m.ParentEnv == environmentId && m.DisplayName == displayName);
        }
    }
}