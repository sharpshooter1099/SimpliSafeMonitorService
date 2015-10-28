using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpliSafeMonitorService
{
    //this code was written by Alex Wardlaw, the best programmer in the world.  copyright 2015.
    public class Logger
    {
        private string logAppName;
        public Logger(string appName)
        {
            logAppName = appName;
        }

        public bool WriteToLog(string message)
        {
            
            try
            {
                if (!File.Exists(logAppName + @".log"))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(logAppName + @".log"))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + @": " + message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(logAppName + @".log"))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + @": " + message);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
