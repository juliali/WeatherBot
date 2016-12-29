using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.LUEngine.Luis;
using WeatherBot.Engine.LUEngines.RulebasedEngine;

namespace WeatherBot.Engine.LUEngines
{
    public class LUController
    {
        private LuisClient luisClient = new LuisClient();
        
        private RulebasedExtractor LocationExtractor = new RulebasedExtractor("WeatherBot.Res.ChineseCities.txt");

        private RulebasedIntentClassifier LocationIC = new RulebasedIntentClassifier("WeatherBot.Res.IntentRules.txt");

        private RulebasedPreprocessor preprocessor = new RulebasedPreprocessor("WeatherBot.Res.Preprocess_replacewords.txt");
        public LUInfo Understand(string utterance)
        {
            string newUtterance = preprocessor.Preprocess(utterance);

            LUInfo info = this.luisClient.Query(newUtterance); // the preprocess is for luis only

            string ruleBasedIntent = LocationIC.GetCategory(utterance);

            if (!string.IsNullOrWhiteSpace(ruleBasedIntent))
            {
                info.Intent.intent = ruleBasedIntent;
                info.Intent.score = 1;
            }

            List<Entity> rulebasedEntities = LocationExtractor.Extract(utterance);                    
                   
            if (rulebasedEntities.Count > 0)
            {
                info.EntityList.AddRange(rulebasedEntities);
            }

            return info;
        }
    }
}
