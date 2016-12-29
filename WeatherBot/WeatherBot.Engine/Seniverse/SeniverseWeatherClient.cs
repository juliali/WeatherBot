using Newtonsoft.Json.Linq;
using System;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Seniverse
{
    public class SeniverseWeatherClient : SeniverseClient
    {        
        private const string rootURL = "https://api.thinkpage.cn/v3/weather";
           
        public string GetRealTimeWeather(string location)
        {
            string currentweatherurl = rootURL + "/now.json?key={0}&location={1}&language=zh-Hans&unit=c";
            string completedURL = string.Format(currentweatherurl, key, location);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);

            dynamic jsonObj = stuff.results[0].now;

            string[] realtimeFieldsToDisplay = { "text", "temperature", "wind_direction", "wind_scale", "humidity" };
            string[] realtimeResponseTemplate = { "{0}", "气温{0}度", "风向{0}", "风力{0}级", "湿度{0}%" };

            string responseText = GetResponseText(jsonObj.ToString(), realtimeFieldsToDisplay, realtimeResponseTemplate);

            responseText = DateTime.Now.ToString(" yyyy-MM-dd ") + responseText;
            return location + ": " + responseText;
        }

        public string GetPredictedWeatherDaily(string location, DateTime startDate, DateTime endDate)//int start, int len)
        {
            DateOffsetInfo dateoffset = FileUtils.GetDateOffset(startDate, endDate);

            
            string dailyweatherurl = rootURL + "/daily.json?key={0}&location={1}&language=zh-Hans&unit=c&start={2}&days={3}";

            string[] FieldsToDisplay = { "date", "text_day", "high", "text_night", "low",  "wind_direction", "wind_scale" };
            string[] ResponseTemplate = { "{0}", "白天{0}", "最高气温{0}度", "夜间{0}", "最低气温{0}度", "风向{0}", "风力{0}级" };

            if (dateoffset.start + dateoffset.len > 15)
            {
                return "我们仅预报未来15天的天气";
            }

            if (dateoffset.start < -1)
            {
                dateoffset.len -= -1 - dateoffset.len;
                dateoffset.start = -1;
            }

            string completedURL = string.Format(dailyweatherurl, key, location, dateoffset.start, dateoffset.len);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);
            dynamic jsonArray = stuff.results[0].daily;

            string respText = "";

            foreach(dynamic jsonObj in jsonArray)
            {
                string resp = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);
                respText += resp + "\r\n";
            }

            return location + ":\r\n" + respText;
        }

        public string GetPredictedWeatherHourly(string location, DateTime startDate, DateTime endDate)
        {
            DateOffsetInfo dateoffset = FileUtils.GetTimeOffset(startDate, endDate);
            int start = dateoffset.start;
            int len = dateoffset.len;

            if (start + len > 24)
            {
                return "我们仅预报24小时之内的天气情况";
            }

            string hourlyweatherurl = rootURL + "/hourly.json?key={0}&language=zh-Hans&location={1}&start={2}&hours={3}";
            //{"time":"2016-12-27T06:00:00+08:00","text":"多云","code":"4","temperature":"-6","humidity":"70","wind_direction":"北","wind_speed":"3.6"}
            string[] FieldsToDisplay = { "time", "text", "temperature",  "wind_direction" };
            string[] ResponseTemplate = { "{0}", "{0}", "气温{0}度",  "风向{0}"};
            
            string completedURL = string.Format(hourlyweatherurl, key, location, start, len);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);
            dynamic jsonArray = stuff.results[0].hourly;

            string respText = "";
            int index = 0;

            foreach (dynamic jsonObj in jsonArray)
            {
                string timeStr = jsonObj.time;

                DateTime time = DateTime.Parse(timeStr);                

                string resp = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);
                respText += resp + "\r\n";
                index++;
            }

            return location + ":\r\n" + respText;
        }

    }
}
