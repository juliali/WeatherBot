using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Seniverse;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class WeatherSrv : IntentSrv
    {
        private SeniverseWeatherClient client = new SeniverseWeatherClient();

        public override string GetAnswer(LUInfo luInfo)
        {
            string response = LuisUtils.GetOutofScopeAnswer();

            string location = LuisUtils.GetLocation(luInfo);

            TimeRange range = LuisUtils.GetTimeRange(luInfo);

            if (range == null)
            {
                response = client.GetRealTimeWeather(location);
            }
            else if (!range.IsHourly)
            {
                response = client.GetPredictedWeatherDaily(location, range.startDate, range.endDate);
            }
            else
            {
                response = client.GetPredictedWeatherHourly(location, range.startDate, range.endDate);
            }

            return response;           
        }
    }
}
