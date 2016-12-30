using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using WeatherBot.Engine.Common;

namespace WeatherBot.Engine.Seniverse
{
    public abstract  class SeniverseClient
    {
        protected readonly APIClient client = new APIClient();
        protected static string key = ConfigurationManager.AppSettings["SeniverseKey"];

        protected string GetResponseText(dynamic jsonObj, string[] fieldsToDisplay, string[] ResponseTemplate)
        {
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)jsonObj);

            string responseText = "";

            for (int i = 0; i < fieldsToDisplay.Length; i++)
            {
                if (!jsonDict.ContainsKey(fieldsToDisplay[i]))
                {
                    continue;
                }

                object obj = jsonDict[fieldsToDisplay[i]];

                string value = obj.ToString();

                Type objType = obj.GetType();
                if (objType.Name == "JArray" )
                {
                    string[] objstrs = ((JArray)obj).ToObject<string[]>();
                    value = string.Join(", ", objstrs);
                }
                    
                responseText += string.Format(ResponseTemplate[i], value);

                if (i < fieldsToDisplay.Length - 1)
                {
                    responseText += ", ";
                }
            }

            return responseText;
        }
        
    }
}
