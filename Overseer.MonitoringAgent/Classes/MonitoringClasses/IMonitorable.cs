using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent
{
    interface IMonitorable
    {
        // snapshot current system status for this monitored component
        void Snapshot();

        // this will be used later in conjuction with monitoring settings for each monitored component
        //void UpdateSettings();
    }
}
