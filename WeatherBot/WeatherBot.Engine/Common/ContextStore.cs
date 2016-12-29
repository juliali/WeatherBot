using System;
using System.Collections.Generic;
using WeatherBot.Engine.Data;

namespace WeatherBot.Engine.Common
{
    public class ContextStore
    {
        private static ContextStore instance;

        private static Dictionary<string, WBContext> contextMap;

        private ContextStore()
        {
            contextMap = new Dictionary<string, WBContext>();
        }

        public static ContextStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContextStore();
                }
                return instance;
            }
        }

        public WBContext GetContext(string userId)
        {
            // If userId is null, set a uniq id for each uatterance.
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = DateTime.Now.Ticks.ToString(); 
            }

            if (contextMap.ContainsKey(userId))
            {
                return contextMap[userId];
            }
            else
            {                
                WBContext newContext = new WBContext(userId);
                contextMap.Add(userId, newContext);

                return newContext;
            }
        }       
    }
}
