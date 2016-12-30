using WeatherBot.Engine.Data;
using WeatherBot.Engine.Seniverse;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class WeatherSrv : IntentSrv
    {
        private SeniverseWeatherClient client = new SeniverseWeatherClient();

        public override string GetAnswer(WBContext context, LUInfo luInfo)
        {
            string response = DatetimeUtils.GetOutofScopeAnswer();

            string location = context.Location;
            TimeRange range = context.timeRange;

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

            return response + "\r\n";           
        }
    }
}
