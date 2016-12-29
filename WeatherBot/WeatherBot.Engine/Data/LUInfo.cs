using System;
using System.Collections.Generic;
using WeatherBot.Engine.LUEngine.Luis;

namespace WeatherBot.Engine.Data
{
    public class LUInfo
    {
        public Intent Intent = new Intent();
        public List<Entity> EntityList = new List<Entity>();

        public void SetIntent(IntentValue intent)
        {
            Intent.intent = intent.Intent;
            Intent.score = intent.Score;
        }

        public void AddEntity(Entity entity)
        {
            EntityList.Add(entity);
        }

        override
        public string ToString()
        {
            string str = "";
            str += Intent.ToString() + "\n";
            foreach(Entity entity in EntityList)
            {
                str += entity.ToString() + "\n";
            }

            return str;
        }

    }

    public class Intent
    {
        public string intent = "None";
        public double score = 0;

        override
        public string ToString()
        {
            string str = "Intent: " + intent + "; Score: " + score.ToString();
            return str;
        }
    }

    public class Entity
    {
        public string value;
        public string type;
        public string resolution;

        override
        public string ToString()
        {
            string str = "Type: " + type + "; Value: " + value + "; Resoluation: " + resolution;
            return str;
        }
    }

    public class TimePeriod
    {
        public int startHour = -1;
        public int endHour = -1;

        public bool IsSingleValue()
        {
            if (startHour == endHour)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class DatePeriod
    {
        public string startDateStr;
        public string endDateStr;

        public bool IsSingleValue()
        {
            if (startDateStr == endDateStr)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
