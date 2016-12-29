using System.Collections.Generic;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Seniverse;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class SmogSrv : IntentSrv
    {
        private SeniverseSmogClient client = new SeniverseSmogClient();

        public override string GetAnswer(LUInfo luInfo)
        {
            string response = LuisUtils.GetOutofScopeAnswer();

            string location = LuisUtils.GetLocation(luInfo);

            TimeRange range = LuisUtils.GetTimeRange(luInfo);

            List<string> stations = LuisUtils.GetEntity(luInfo, "Station");

            string station = null;

            if (stations != null && stations.Count > 0)
            {
                station = stations[0];
            }

            if (range == null)
            {
                response = client.GetRealTimeAirQuality(location, station);
            }
            else if (!range.IsHourly)
            {
                response = client.GetPredictedSmogDaily(location, range.startDate, range.endDate);
            }
            else
            {
                response = client.GetPredictedSmogHourly(location, range.startDate, range.endDate);
            }

            return response;
        }
    }
}
