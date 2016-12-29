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
    public class RestrictedDrivingSrv : IntentSrv
    {
        private SeniverseRestrictedDrivingClient client = new SeniverseRestrictedDrivingClient();

        public override string GetAnswer(LUInfo luInfo)
        {
            string response = LuisUtils.GetOutofScopeAnswer();

            string location = LuisUtils.GetLocation(luInfo);

            TimeRange range = LuisUtils.GetTimeRange(luInfo);

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
