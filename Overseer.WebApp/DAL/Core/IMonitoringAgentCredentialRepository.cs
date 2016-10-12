using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IMonitoringAgentCredentialRepository : IRepository<MonitoringAgentCredential>
    {
        // overloaded get method (since we need to pass in Guid, rather than int)
        MonitoringAgentCredential Get(Guid machineId);
    }
}
