using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.LUEngines.RulebasedEngine
{
    public class RulebasedExtractor
    {
        private List<ExtractorRuleInfo> Rules;

        public RulebasedExtractor(string ResourceName)
        {           
            this.Rules = FileUtils.ReadExtractorInfo(ResourceName);
        }

        public List<Entity> Extract(string utterance)
        {
            List<Entity> entities = new List<Entity>();

            foreach(ExtractorRuleInfo Rule in Rules)
            {
                List<string> matchedValues = FileUtils.Match(Rule.ValueRegex, utterance);

                if (matchedValues.Count > 0)
                {
                    foreach(string matchedValue in matchedValues)
                    {
                        Entity entity = new Entity();
                        entity.value = matchedValue;
                        entity.type = Rule.Type;

                        if (!string.IsNullOrWhiteSpace(Rule.ResolutionValue))
                        {
                            entity.resolution = Rule.ResolutionValue;
                        }

                        entities.Add(entity);
                    }
                }
            }

            return entities;
        }
    }
}
