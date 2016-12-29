using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.LUEngines.RulebasedEngine
{
    public class RulebasedIntentClassifier
    {
        private List<ExtractorRuleInfo> Rules;

        public RulebasedIntentClassifier(string ResourceName)
        {
            this.Rules = FileUtils.ReadExtractorInfo(ResourceName);
        }

        public string GetCategory(string utterance)
        {
            foreach(ExtractorRuleInfo Rule in Rules)
            { 
                List<string> matchedValues = FileUtils.Match(Rule.ValueRegex, utterance);

                if (matchedValues.Count > 0)
                {
                    return Rule.Type;
                }
            }

            return null;
        }
    }
}
