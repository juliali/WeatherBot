using System;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Seniverse;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class CarWashingSrv : IntentSrv
    {
        private SeniverseLivingClient client = new SeniverseLivingClient();

        public override string GetAnswer(WBContext context, LUInfo luInfo)
        {
            string location = context.Location;
            TimeRange timeRange = context.timeRange;
            
            if (timeRange != null && timeRange.startDate > DateTime.Now)
            {
                return "我们仅提供今天的洗车指数";
            }

            string response = client.GetSuggestion(location, LifeSuggestionType.CarWashing);

            return response;

        }        
    }
}
