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
    }
}
