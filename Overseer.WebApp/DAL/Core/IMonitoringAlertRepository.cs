using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.WebApp.DAL.Core
{
    public interface IMonitoringAlertRepository : IRepository<MonitoringAlert>
    {
        IEnumerable<MonitoringAlert> GetAllAlertsByMachine(Guid machineId);

        IEnumerable<MonitoringAlert> GetActiveAlertsByMachine(Guid machineId);

        IEnumerable<MonitoringAlert> GetHistoricalAlertsByMachine(Guid machineId);

        void DeleteAlertsByMachine(Guid machineId);

        int GetAlertCount(int userId);

        int GetWarningCount(int userId);

        int GetAlertAndWarningCount(int userId);

        IEnumerable<MonitoringAlert> GetMostRecentAlerts(int userId, int skip, int range, int? sev);

        int AlertFilterQueryCount(int userId, bool archived, int? sev, int? envId, Guid? machineId);

        IEnumerable<MonitoringAlert> AlertFilterQuery(int userId, int skip, int range, bool archived, int? severity, int? envId, Guid? machineId);
    }
}
