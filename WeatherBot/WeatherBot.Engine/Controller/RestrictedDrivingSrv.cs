using WeatherBot.Engine.Data;
using WeatherBot.Engine.Seniverse;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class RestrictedDrivingSrv : IntentSrv
    {
        private SeniverseRestrictedDrivingClient client = new SeniverseRestrictedDrivingClient();

        public override string GetAnswer(WBContext context, LUInfo luInfo)
        {
            string response = DatetimeUtils.GetOutofScopeAnswer();

            string location = context.Location;
            TimeRange range = context.timeRange;

            if (range == null)
            {
                response = client.GetRestrictedNumber(location, null, null);
            }
            else
            {
                response = client.GetRestrictedNumber(location, range.startDate, range.endDate);
            }

            return response;
        }
    
    }
}
