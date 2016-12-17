using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface ITestEnvironmentRepository : IRepository<TestEnvironment>
    {
        // get environment and include monitoring settings
        TestEnvironment GetWithMonitoringSettings(int id);

        // get environment and include all related values
        TestEnvironment GetWithAllRelatedValues(int id);

        // get all data necessary for 'Environmentseer' page
        TestEnvironment GetEnvironmentseerData(int id);

        // get all data for monitoring summary partial view loaded within 'Environmentseer' page
        TestEnvironment GetEnvironmentMonitoringSummaryData(int id);

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
