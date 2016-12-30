using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBot.Engine.Utils
{
    public class LogUtils
    {
        private const string logfilename = "d:\\home\\site\\wwwroot\\bin\\wbservice_log.txt";
        public static void Log(string logMessage)
        {
            Log(logMessage, logfilename);
        }

        public static void Log(DateTime startTime, DateTime endTime, string msg)
        {
            TimeSpan latency = endTime.Subtract(startTime);
            double ms = latency.TotalMilliseconds;

            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "";
            }

            Log(msg + ": It took " + ms.ToString() + " MS to get answer.");

        }

        private static void Log(string logMessage, string fileName)
        {
            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine("\r\n{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logMessage);
            }
        }
    }
}
