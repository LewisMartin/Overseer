using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IProcessSettingsRepository : IRepository<ProcessSettings>
    {
        ProcessSettings Get(Guid machineId, string processName);

        IEnumerable<ProcessSettings> GetByMachine(Guid machineId);

        void DeleteByMachine(Guid machineId);
    }
}
