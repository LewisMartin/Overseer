using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envSeer.DAL.Core
{
    public interface ITestEnvironmentRepository : IRepository<TestEnvironment>
    {
        // get environment by creator (userId)
        IEnumerable<TestEnvironment> GetEnvironmentsByCreator(int userId);

        // get environment by creator (userId)
        IEnumerable<TestEnvironment> GetEnvironmentsAndChildMachinesByCreator(int userId);

        // get environment by creator & environment name (used to validate against duplication)
        TestEnvironment GetEnvironmentByCreatorAndName(int userId, string name);

        // check if an environment name has already been used by a user
        bool CheckEnvironmentExistsByCreatorAndName(int userId, string name);
    }
}
