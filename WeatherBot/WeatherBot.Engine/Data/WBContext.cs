using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBot.Data
{
    public class WBContext
    {
        public DateTime validTime;
        public string userId;

        public string Intent;
        public string Location;
        public DateTime startDate;
        public DateTime endDate;

        public WBContext(string timestr, string userId)
        {
            this.validTime = DateTime.Parse(timestr);
            this.userId = userId;
        }

        public bool IsValid()
        {
            DateTime nowTime = DateTime.Now;

            if (nowTime.Subtract(validTime).TotalMinutes > 30)
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
