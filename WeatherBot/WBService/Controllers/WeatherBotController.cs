using System.Web.Http;
using WeatherBot.Engine.Controller;

namespace WBService.Controllers
{
    public class WeatherBotController : ApiController
    {
        
        private IntentController bot = new IntentController();

        // GET api/weatherbot?query={query}&userId={userId}
        public string Get([FromUri]string query, [FromUri]string userId = null)
        {
            string answer = bot.Answer(userId, query);
            return answer;
        }
    }
}