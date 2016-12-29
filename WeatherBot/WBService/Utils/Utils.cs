using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WBService.Utils
{
    public class Utils
    {
        private const string logfilename = "d:\\home\\site\\wwwroot\\bin\\wbservice_log.txt";
        public static void Log(string logMessage)
        {
            Log(logMessage, logfilename);
        }

        private static void Log(string logMessage, string fileName)
        {
            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine("\r\n{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logMessage);
            }
        }

        public static string GetDefaultAnswer()
        {
            return "我叫悦心，还是个baby，现在只能帮您查中国城市的天气、空气质量、车辆限行，洗车和穿衣指数。\r\n我妈妈悦思悦读有时会发表一些她的思考，大部分是关于职场、大数据和机器学习的。欢迎关注。";
            //"本公号发布IT人的阅读与思考，间或有技术性文章，涉及职场、数据分析、数据挖掘、机器学习、大数据、分布式计算框架等，欢迎大家订阅";
        }
    }
}