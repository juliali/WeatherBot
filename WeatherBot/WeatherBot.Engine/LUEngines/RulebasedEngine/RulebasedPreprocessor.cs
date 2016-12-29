using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.LUEngines.RulebasedEngine
{
    public class RulebasedPreprocessor
    {
        private List<PreprocessRuleInfo> Rules;

        public RulebasedPreprocessor(string ResourceName)
        {
            this.Rules = FileUtils.ReadPreprocessInfo(ResourceName);
        }

        public string Preprocess(string utterance)
        {
            string newUtterance = utterance;

            if (string.IsNullOrWhiteSpace(utterance))
            {
                return newUtterance;
            }

            foreach(PreprocessRuleInfo info in Rules)
            {
                newUtterance = FileUtils.ReplaceRegx(info, newUtterance);
            }

            return newUtterance;
        }
    }
}
