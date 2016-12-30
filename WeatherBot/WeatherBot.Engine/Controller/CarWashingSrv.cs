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
                       
            string response = client.GetSuggestion(location, LifeSuggestionType.CarWashing);

            if (timeRange != null && timeRange.startDate > DateTime.Now)
            {
                response += "\r\n我们仅提供今天的洗车指数\r\n";
            }

            return response;

        }        
    }
}
