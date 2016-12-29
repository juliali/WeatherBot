using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (contextMap.ContainsKey(userId))
            {
                return contextMap[userId];
            }
            else
            {
                return null;
            }
        }

        public void SetContext(string userId, WBContext context)
        {
            if (contextMap.ContainsKey(userId))
            {
                contextMap[userId] = context;
            }
            else
            {
                contextMap.Add(userId, context);
            }
        }
    }
}
