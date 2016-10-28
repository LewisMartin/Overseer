using Overseer.DTOs.MonitoringAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent
{
    // generic interface to declare mandatory functionality of monitor classes.
    interface IMonitorable<T> where T : MonitoredInformation
    {
        // snapshot current system status for this monitored component
        void Snapshot();

        // package monitored information into format to be serialized to json and sent to server
        T GetDTO();

        // this will be used later in conjuction with monitoring settings for each monitored component
        //void UpdateSettings();
    }
}
