using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WeatherBot.Engine.Controller;

namespace WBService.Controllers
{
    public class WeatherBotController : ApiController
    {
        
        private IntentController bot = new IntentController();

        // GET api/weatherbot?query={query}
        public string Get([FromUri]string query)
        {
            string answer = bot.Answer(query);
            return answer;
        }
    }
}