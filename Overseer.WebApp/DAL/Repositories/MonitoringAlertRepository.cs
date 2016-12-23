using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class MonitoringAlertRepository : Repository<MonitoringAlert>, IMonitoringAlertRepository
    {
        public MonitoringAlertRepository(OverseerDBContext context) : base(context) { }

        public IEnumerable<MonitoringAlert> GetAllAlertsByMachine(Guid machineId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.MachineId == machineId).ToList();
        }

        public IEnumerable<MonitoringAlert> GetActiveAlertsByMachine(Guid machineId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.MachineId == machineId && a.Historical == false).ToList();
        }

        public IEnumerable<MonitoringAlert> GetHistoricalAlertsByMachine(Guid machineId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.MachineId == machineId && a.Historical == true).ToList();
        }

        public void DeleteAlertsByMachine(Guid machineId)
        {
            dbContext.MonitoringAlerts.RemoveRange(GetAllAlertsByMachine(machineId));
        }
    }
}