namespace WeatherBot.Engine.LUEngine.Luis
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class QueryResult
    {
        public string Query { get; set; }

        public List<IntentValue> Intents { get;  set; }

        public List<EntityValue> Entities { get;  set; }
    }

    public class IntentValue
    {
        public string Intent { get;  set; }

        public double Score { get;  set; }
    }

    public class EntityValue
    {
        public string Entity { get;  set; }

        public string Type { get;  set; }

        public int StartIndex { get;  set; }

        public int EndIndex { get;  set; }

        public double Score { get;  set; }

        public BuiltInDateTimeResolution resolution { get;  set; }
      
    }

    public class BuiltInDateTimeResolution
    {
        public string date;
        public string time;
    }
}