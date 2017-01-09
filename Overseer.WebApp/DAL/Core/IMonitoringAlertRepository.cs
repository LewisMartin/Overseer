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

        IEnumerable<MonitoringAlert> GetMostRecentAlerts(int userId, int range, int? sev);

        IEnumerable<MonitoringAlert> AlertFilterQuery(int userId, bool archived, int? severity, int? envId, Guid? machineId);
    }
}
