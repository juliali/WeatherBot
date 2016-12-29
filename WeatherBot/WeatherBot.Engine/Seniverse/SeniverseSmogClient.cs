using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Seniverse
{
    public class SeniverseSmogClient : SeniverseClient
    {
        private const string rootURL = "https://api.thinkpage.cn/v3/air";

        public string GetRealTimeAirQuality(string location, string station)
        {            
            string url = rootURL + "/now.json?key={0}&location={1}&language=zh-Hans&scope=all";

            if (string.IsNullOrWhiteSpace(station))
            {
                url = rootURL + "/now.json?key={0}&location={1}&language=zh-Hans&scope=city";
            }

            string completedURL = string.Format(url, key, location);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);

            dynamic jsonObj = stuff.results[0].air.city;

            string[] FieldsToDisplay = { "quality", "aqi", "pm25", "pm10" };
            string[] ResponseTemplate = { "空气质量{0}", "空气质量指数 {0}", "PM2.5 {0}", "PM10 {0}"};


            string CityResponseText = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);

            CityResponseText = DateTime.Now.ToString(" yyyy-MM-dd ") + CityResponseText;
            if (string.IsNullOrWhiteSpace(station))
            {                
                return location + ": " + CityResponseText;
            }
            else
            {
                dynamic stationArray = stuff.results[0].air.stations;
                foreach(dynamic stationObj in stationArray)
                {
                    string realStationName = stationObj.station.ToString();
                    if (station.Contains(realStationName))
                    {
                        string StationResponseText = GetResponseText(stationObj.ToString(), FieldsToDisplay, ResponseTemplate);
                        return realStationName + ": " + StationResponseText;
                    }
                }
                return CityResponseText;
            }
            
        }

        public string GetPredictedSmogDaily(string location, DateTime startDate, DateTime endDate)
        {
            DateOffsetInfo dateoffset = FileUtils.GetDateOffset(startDate, endDate);
            int start = dateoffset.start;
            int len = dateoffset.len;

            string dailyweatherurl = rootURL + "/daily.json?key={0}&language=zh-Hans&location={1}&days={2}";

            string[] FieldsToDisplay = { "date", "quality", "aqi", "pm25", "pm10" };
            string[] ResponseTemplate = { "{0}", "空气质量{0}", "空气质量指数 {0}", "PM2.5 {0}", "PM10 {0}" };

            int days = start + len;

            if (days > 7)
            {
                return "仅提供7天之内的空气质量预报";
            }

            string completedURL = string.Format(dailyweatherurl, key, location, days);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);
            dynamic jsonArray = stuff.results[0].daily;

            string respText = "";
            int index = 0;
            foreach (dynamic jsonObj in jsonArray)
            {
                if (index < start)
                {
                    index++;
                    continue;
                }
                else if (index >= start + len)
                {
                    break;
                }

                string resp = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);
                respText += resp + "\r\n";
                index++;
            }

            return location + ":\r\n" + respText;
        }

        public string GetPredictedSmogHourly(string location, DateTime startDate, DateTime endDate)
        {
            DateOffsetInfo dateoffset = FileUtils.GetDateOffset(startDate, endDate);
            int start = dateoffset.start;
            int len = dateoffset.len;

            string hourlyweatherurl = rootURL + "/hourly.json?key={0}&language=zh-Hans&location={1}&days={2}";

            string[] FieldsToDisplay = { "time", "quality", "aqi", "pm25", "pm10" };
            string[] ResponseTemplate = { "{0}", "空气质量{0}", "空气质量指数 {0}", "PM2.5 {0}", "PM10 {0}" };

            int days = start + len;

            if (days > 7)
            {
                return "仅提供7天之内的空气质量预报";
            }

            string completedURL = string.Format(hourlyweatherurl, key, location, days);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);
            dynamic jsonArray = stuff.results[0].hourly;

            string respText = "";            
            List<string> respList = new List<string>();

            foreach (dynamic jsonObj in jsonArray)
            {                
                DateTime date = DateTime.Parse(jsonObj.time.ToString());

                    if (date < startDate || date >= endDate)
                    {
                        continue;
                    }
                

                string resp = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);
                respList.Add(resp);                
            }

            if (respList.Count == 0)
            {
                respText = "我们仅提供七天之内的空气质量预报";
            }
            else
            {
                respText = string.Join("\r\n", respList.ToArray());
            }

            return location + ":\r\n" + respText;
        }
    }
}
