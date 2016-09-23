using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envSeer.DAL.Core
{
    public interface IMachineRepository : IRepository<Machine>
    {
        // get a machine and eager load it's environment
        Machine GetMachineAndParent(int machineId);

        // get machine by environment Id & machine display name
        Machine GetMachineByEnvironmentAndDisplayName(int environmentId, string displayName);

        // return whether or not a certain machine exists for a given environment
        bool CheckMachineExistsByEnvironmentAndDisplayName(int environmentId, string displayName);
    }
}
