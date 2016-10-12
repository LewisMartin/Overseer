using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class MonitoringAgentCredentialRepository : Repository<MonitoringAgentCredential>, IMonitoringAgentCredentialRepository
    {
        public MonitoringAgentCredentialRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public MonitoringAgentCredential Get(Guid machineId)
        {
            return dbContext.MonitoringAgentCredentials.FirstOrDefault(c => c.MachineID == machineId);
        }
    }
}