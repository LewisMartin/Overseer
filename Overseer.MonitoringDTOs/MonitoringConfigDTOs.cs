using System;

namespace Overseer.DTOs.MonitoringConfig
{
    // DTO for sending request for machine secret generation
    public class MonitoringAgentConfigRequest
    {
        public Guid MachineGuid { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }

    // DTO for returning monitoring config to agent (incl machine secret)
    public class MonitoringAgentConfigResponse
    {
        public bool Success { get; set; }

        public string MachineSecret { get; set; }
    }
}