using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

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
            return dbContext.MonitoringAlerts.Where(a => a.MachineId == machineId && a.Historical == false).OrderBy(a => a.Severity).ToList();
        }

        public IEnumerable<MonitoringAlert> GetHistoricalAlertsByMachine(Guid machineId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.MachineId == machineId && a.Historical == true).ToList();
        }

        public void DeleteAlertsByMachine(Guid machineId)
        {
            dbContext.MonitoringAlerts.RemoveRange(GetAllAlertsByMachine(machineId));
        }

        public IEnumerable<MonitoringAlert> GetMostRecentAlerts(int userId, int skip, int range, int? sev)
        {
            return dbContext.MonitoringAlerts.Include(a => a.Machine.TestEnvironment)
                                             .Where(a => a.Machine.TestEnvironment.Creator == userId
                                                        && a.Archived == false
                                                        && a.Severity == (sev != null ? sev : a.Severity))
                                             .OrderByDescending(a => a.AlertCreationTime)
                                             .Skip(skip)
                                             .Take(range);
        }

        public int GetAlertAndWarningCount(int userId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.Archived == false).Count();
        }

        public int GetAlertCount(int userId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.Archived == false && a.Severity == 1).Count();
        }

        public int GetWarningCount(int userId)
        {
            return dbContext.MonitoringAlerts.Where(a => a.Archived == false && a.Severity == 0).Count();
        }

        public int AlertFilterQueryCount(int userId, bool archived, int? sev, int? envId, Guid? machineId)
        {
            var alerts = dbContext.MonitoringAlerts.Include(a => a.Machine.TestEnvironment).Where(a => a.Machine.TestEnvironment.Creator == userId && a.Archived == archived).AsQueryable();

            if (sev != null || envId != null || machineId != null)
            {
                return alerts.Where(m => m.Severity == ((sev != null && archived == false) ? sev : m.Severity)
                                            && m.Machine.TestEnvironment.EnvironmentID == (envId != null ? envId : m.Machine.TestEnvironment.EnvironmentID)
                                            && m.MachineId == (machineId != null ? machineId : m.MachineId)).Count();
            }
            else
            {
                return alerts.Count();
            }
        }

        public IEnumerable<MonitoringAlert> AlertFilterQuery(int userId, int skip, int range, bool archived, int? sev, int? envId, Guid? machineId)
        {
            var alerts = dbContext.MonitoringAlerts.Include(a => a.Machine.TestEnvironment).Where(a => a.Machine.TestEnvironment.Creator == userId && a.Archived == archived).AsQueryable();

            if (sev != null || envId != null || machineId != null)
            {
                return alerts.Where(m => m.Severity == ((sev != null && archived == false) ? sev : m.Severity)
                                            && m.Machine.TestEnvironment.EnvironmentID == (envId != null ? envId : m.Machine.TestEnvironment.EnvironmentID)
                                            && m.MachineId == (machineId != null ? machineId : m.MachineId))
                                            .OrderByDescending(a => a.AlertCreationTime)
                                            .Skip(skip)
                                            .Take(range)
                                            .ToList();
            }
            else
            {
                return alerts.OrderByDescending(a => a.AlertCreationTime)
                            .Skip(skip)
                            .Take(range)
                            .ToList()
                            .AsEnumerable();
            }
        }
    }
}