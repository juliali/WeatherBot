using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBot.Engine.Data
{
    public class ExtractorRuleInfo
    {
        public string ValueRegex;
        public string Type;
        public string ResolutionValue;
    }

    public class PreprocessRuleInfo
    {
        public string ValueRegex;
        public string OrigValue;
        public string NewValue;        
    }
}
