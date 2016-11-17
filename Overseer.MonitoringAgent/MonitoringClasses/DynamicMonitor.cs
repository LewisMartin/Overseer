using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public abstract class DynamicMonitor : BaseMonitor
    {
        protected List<string> MonitoredEntities;

        public DynamicMonitor() { }

        public void UpdateMonitoredEntities(List<string> updatedEntities)
        {
            MonitoredEntities = updatedEntities;
        }
    }
}
