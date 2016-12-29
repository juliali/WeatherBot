using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WeatherBot.Engine.Seniverse
{
    public class SeniverseRestrictedDrivingClient : SeniverseClient
    {
        private const string rootURL = "https://api.thinkpage.cn/v3/life";

        public string GetRestrictedNumber(string location, DateTime? startDate, DateTime? endDate)
        {
            string url = rootURL + "/driving_restriction.json?key={0}&location={1}";
            string completedURL = string.Format(url, key, location);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);

            dynamic jsonObj = stuff.results[0].restriction;
            dynamic jsonArray = jsonObj.limits;

            if (jsonArray == null)
            {
                return "没有" + location + "限行计划数据";
            }
            
            string[] FieldsToDisplay = { "date", "plates", "memo" };
            string[] ResponseTemplate = { "{0}", "限行尾号{0}", "{0}" };
            
            string ResponseText = "";
            
            List<string> respList = new List<string>();
            foreach (dynamic jsonLimit in jsonArray)
            {               
                DateTime date = DateTime.Parse(jsonLimit.date.ToString());

                if (startDate != null && endDate != null)
                {
                    if (date < startDate || date > endDate)
                    {
                        continue;
                    }
                }
                string resp = location + ": " + GetResponseText(jsonLimit.ToString(), FieldsToDisplay, ResponseTemplate);
                respList.Add(resp);
                
            }

            if (respList.Count == 0)
            {
                ResponseText = "我们仅提供三天之内的限行计划";
            }
            else
            {
                ResponseText = string.Join("\r\n", respList.ToArray());
            }

            return ResponseText;
        }
    }
}
