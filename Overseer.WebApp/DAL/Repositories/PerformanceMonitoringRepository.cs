using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class PerformanceMonitoringRepository : Repository<PerformanceInfo>, IPerformanceMonitoringRepository
    {
        public PerformanceMonitoringRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public PerformanceInfo GetLatestReading(Guid machineId)
        {
            return dbContext.PerformanceMonitoring.Where(m => m.MachineID == machineId).OrderByDescending(m => m.ReadingNumber).FirstOrDefault();
        }

        public List<PerformanceInfo> GetAllReadingsForMachine(Guid machineId)
        {
            return dbContext.PerformanceMonitoring.Where(m => m.MachineID == machineId).ToList();
        }

        public List<PerformanceInfo> GetOrderedReadingsForMachine(Guid machineId)
        {
            return dbContext.PerformanceMonitoring.Where(m => m.MachineID == machineId).OrderByDescending(m => m.ReadingNumber).ToList();
        }
    }
}