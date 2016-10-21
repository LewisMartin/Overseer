using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Overseer.MonitoringAgent
{
    public class MonAgentConfig
    {
        public string AppUri { get; set; }

        public string MachineGuid { get; set; }

        public string MachineSecret { get; set; }

        private string ConfigFileLocation;

        public MonAgentConfig(string configFileLocation)
        {
            ConfigFileLocation = configFileLocation;
        }

        // load the configuration values from the xml config file
        public void LoadConfig()
        {
            using (XmlReader xmlReader = XmlReader.Create(ConfigFileLocation))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        switch (xmlReader.Name.ToString())
                        {
                            case "MachineGUID":
                                MachineGuid = xmlReader.ReadString();
                                break;
                            case "MachineSecret":
                                MachineSecret = xmlReader.ReadString();
                                break;
                            case "ServerAddress":
                                AppUri = xmlReader.ReadString();
                                break;
                        }
                    }
                }
            }
        }
    }
}
