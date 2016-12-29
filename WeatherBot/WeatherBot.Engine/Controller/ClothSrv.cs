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
    public class ClothSrv : IntentSrv
    {
        private SeniverseLivingClient client = new SeniverseLivingClient();
        public override string GetAnswer(LUInfo luInfo)
        {
            string location = LuisUtils.GetLocation(luInfo);

            DatePeriod datePeriod = LuisUtils.GetDatePeriod(luInfo);

            if (datePeriod != null && DateTime.Parse(datePeriod.startDateStr) > DateTime.Now)
            {
                return "我们仅提供今天的穿衣指数";
            }

            string response = client.GetSuggestion(location, LifeSuggestionType.Cloth);

            return response;
        }
    }
}
