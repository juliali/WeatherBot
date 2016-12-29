using Newtonsoft.Json.Linq;

namespace WeatherBot.Engine.Seniverse
{

    public enum LifeSuggestionType
    {
        Cloth, CarWashing
    }

    public class SeniverseLivingClient : SeniverseClient
    {
        private const string rootURL = "https://api.thinkpage.cn/v3/life/";

        public string GetSuggestion(string location, LifeSuggestionType type)
        {

            string url = rootURL + "suggestion.json?key={0}&location={1}&language=zh-Hans";
            string completedURL = string.Format(url, key, location);

            string responseMessage = client.Query(completedURL);

            dynamic stuff = JObject.Parse(responseMessage);

            dynamic jsonObj = stuff.results[0].suggestion.dressing;

            if (type == LifeSuggestionType.CarWashing)
            {
                jsonObj = stuff.results[0].suggestion.car_washing;
            }

            string[] FieldsToDisplay = { "details" };
            string[] ResponseTemplate = { "今天{0}"};


            string ResponseText = GetResponseText(jsonObj.ToString(), FieldsToDisplay, ResponseTemplate);

            return location + ResponseText;            
        }

    }
}
