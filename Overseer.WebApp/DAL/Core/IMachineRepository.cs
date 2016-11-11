using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IMachineRepository : IRepository<Machine>
    {
        // we need to overload the generic get method here (machines table uses GUID as primary key)
        Machine Get(Guid machineId);

        // get a machine and eager load it's environment
        Machine GetMachineAndParent(Guid machineId);

        // get a machine and eager load it's owner via parent environment property
        Machine GetMachineAndOwner(Guid machineId);

        // get machine by environment Id & machine display name
        Machine GetMachineByEnvironmentAndDisplayName(int environmentId, string displayName);

        // get all data to be used on 'Machineseer' page
        Machine GetMachineseerDataMonitored(Guid machineId);

        // return whether or not a certain machine exists for a given environment
        bool CheckMachineExistsByEnvironmentAndDisplayName(int environmentId, string displayName);
    }
}
