using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.Helpers
{
    public class Logger
    {
        private static Logger _LoggerInstance;
        private static string _LogFilePath;
        private static object _lock = new Object();

        private Logger()
        {
            _LogFilePath = @"C:\MonitoringAgentTest\Log.txt";
        }

        public static Logger Instance()
        {
            if(_LoggerInstance == null)
            {
                _LoggerInstance = new Logger();
            }

            return _LoggerInstance;
        }

        public void Log(string msg)
        {
            // lock the streamwriter to prevent multiple threads attempting to access file at once.
            lock (_lock)
            {
                using (StreamWriter sw = new StreamWriter(_LogFilePath, true))
                {
                    if (!File.Exists(_LogFilePath))
                    {
                        File.Create(_LogFilePath);
                        sw.WriteLine(DateTime.Now + " | Log file created.");
                    }

                    sw.WriteLine(DateTime.Now + " | " + msg);
                    sw.Close();
                }
            }
        }
    }
}
