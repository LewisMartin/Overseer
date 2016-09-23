﻿using envSeer.DAL.Core;
using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace envSeer.DAL.Repositories
{
    public class MachineRepository : Repository<Machine>, IMachineRepository
    {
        public MachineRepository(envSeerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public bool CheckMachineExistsByEnvironmentAndDisplayName(int environmentId, string displayName)
        {
            return dbContext.Machine.Any(m => m.ParentEnv == environmentId && m.DisplayName == displayName);
        }

        public Machine GetMachineAndParent(int machineId)
        {
            return dbContext.Machine.Include(m => m.TestEnvironment).Include(m => m.OperatingSys).FirstOrDefault(m => m.MachineID == machineId);
        }

        public Machine GetMachineByEnvironmentAndDisplayName(int environmentId, string displayName)
        {
            return dbContext.Machine.FirstOrDefault(m => m.ParentEnv == environmentId && m.DisplayName == displayName);
        }
    }
}