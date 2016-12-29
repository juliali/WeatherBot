using System;
using System.IO;
using System.Text;
using WeatherBot.Engine.Controller;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.LUEngine.Luis;
using WeatherBot.Engine.Seniverse;

namespace WeatherBot.Engine.Test
{
    class Program
    {
        public static void TestWeatherAPI()
        {
            SeniverseWeatherClient client = new SeniverseWeatherClient();
            string resp = client.GetRealTimeWeather("北京");

            Console.WriteLine(resp);
        }

        public static void TestLuis()
        {
            LuisClient client = new LuisClient();

            string query = "周末天气好吗";
            LUInfo result = client.Query(query);

            Console.WriteLine(result.ToString());
        }

        public static void DateTest()
        {
            string str = "2016-12-16T14:00:00+08:00";
            DateTime date = DateTime.Parse(str);

            Console.WriteLine(date.ToString());
        }

        public static void En2EndTest(string utterance)
        {
            IntentController controller = new IntentController();
            string resp = controller.Answer(null, utterance);

            Console.WriteLine(resp);
        }

        public static void End2EndTestForFile(string inputfile, string outputfile)
        {
            IntentController controller = new IntentController();
            string[] lines = File.ReadAllLines(inputfile);
            int index = 1;
            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                foreach(string question in lines)
                {
                    string answer = controller.Answer(index.ToString(), question);
                    w.WriteLine("[Question]: " + question);
                    w.WriteLine("[Answer]:   " + answer);
                    w.WriteLine("\r\n");

                    index++;
                }
            }
        }

        public static void Main(string[] args)
        {
            /*
            En2EndTest("北京天气？");
            En2EndTest("明天有雾霾吗？");
            En2EndTest("后天适合洗车吗？");
            En2EndTest("出门穿什么合适？");
            En2EndTest("周末还有雾霾吗？");
            En2EndTest("下周一空气质量如何？");
            En2EndTest("周五上午有雾霾吗？");
            En2EndTest("今天适合洗车吗？");
            En2EndTest("今天限行尾号多少？");
            En2EndTest("周四几号限行？");
            En2EndTest("周末限行吗？");
            En2EndTest("明天天津限行尾号多少？");
            En2EndTest("明天保定限行尾号多少？");
            En2EndTest("周六下午3点到5点空气质量怎么样？");
            En2EndTest("下个周末有雾霾吗？");

            En2EndTest("明天天津限行尾号多少？");
            En2EndTest("下周北京空气质量");
            En2EndTest("下周一至周三空气质量");
            En2EndTest("下周三三亚下雨吗");
            En2EndTest("12月28日三亚下雨吗");
            En2EndTest("山东济南");
            En2EndTest("崇礼周日");
            En2EndTest("12月28日三亚下雨吗");
            En2EndTest("明天上午天气");
            En2EndTest("明早天气");
            */

            End2EndTestForFile("D:\\Github\\WeatherBot\\WeatherBot\\WeatherBot.Engine.Test\\files\\test1.txt", "D:\\Github\\WeatherBot\\WeatherBot\\WeatherBot.Engine.Test\\files\\output1.txt");

            Console.WriteLine("Finished!");
        }
    }
}
