using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WeatherBot.Engine.Common;
using WeatherBot.Engine.Data;

namespace WeatherBot.Engine.LUEngine.Luis
{
    public class LuisClient
    {
        private readonly string DefaultAppId = "b935ef33-ebea-489f-82d9-c69e5bca4a26";
        private readonly string DefaultSubscriptionKey = "8150ce0230174de6960e94f92a1d6f2b";

        private readonly APIClient client = new APIClient();
        private string rootURL = "https://api.projectoxford.ai/luis/v1/application";

        public LuisClient()
        {
            this.AppId = DefaultAppId;
            this.SubscriptionKey = DefaultSubscriptionKey;
        }

        public LuisClient(string appId, string subscriptionKey)
        {            
            this.AppId = appId;
            this.SubscriptionKey = subscriptionKey;
        }

        public string AppId { get; }

        public string SubscriptionKey { get; }

        public LUInfo Query(string query)
        {
            string encodedQuestion = HttpUtility.UrlEncode(query, System.Text.Encoding.UTF8);
            string url = rootURL + "?id=" + this.AppId + "&subscription-key=" + this.SubscriptionKey + "&q=" + encodedQuestion; 
            var jsonString = client.Query(url);
            QueryResult result =  JsonConvert.DeserializeObject<QueryResult>(jsonString);

            LUInfo luInfo = GetLUInfo(result);

            return luInfo;
        }

        private LUInfo GetLUInfo(QueryResult result)
        {
            LUInfo luInfo = new LUInfo();

            luInfo.SetIntent(result.Intents[0]);

            if (result.Entities != null)
            { 
                foreach(EntityValue entityValue in result.Entities)
                {
                    Entity entity = new Entity();
                    entity.type = entityValue.Type;
                    entity.value = entityValue.Entity;

                    if (entityValue.resolution != null)
                    {
                        string resolutionValue = null;

                        if (!string.IsNullOrWhiteSpace(entityValue.resolution.date))
                        {
                            resolutionValue = entityValue.resolution.date;
                        }
                        else
                        {
                            resolutionValue = entityValue.resolution.time;
                        }
                        entity.resolution = resolutionValue;
                    }

                    luInfo.AddEntity(entity);
                }
            }

            return luInfo;
        }
    }
}
