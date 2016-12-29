using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Data
{
    public class WBContext
    {
        public DateTime validTime;
        public string userId;

        public string Intent;
        public string Location;
        public TimeRange timeRange;        

        public WBContext(string userId)
        {            
            this.userId = userId;
        }

        public bool IsValid()
        {
            DateTime nowTime = DateTime.Now;

            if (validTime == null)
            {
                return false;
            }
            else if (nowTime.Subtract(validTime).TotalMinutes > 30)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Location))
            {
                return false;
            }
            else
            {
                return true;
            }
        }                
    }
}
