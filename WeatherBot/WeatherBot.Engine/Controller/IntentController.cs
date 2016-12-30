using System;
using System.Collections.Generic;
using WeatherBot.Engine.Common;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.LUEngines;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class IntentController
    {
        private List<IntentSrv> srvs;
        private LUController luController = new LUController();
        private ContextStore contextStore = ContextStore.Instance;

        private readonly HashSet<string> ValidIntent = new HashSet<string> {"Weather", "Smog", "DefaultIntent", "CarWashing", "RestrictedDriving", "Cloth" };

        public IntentController()
        {
            this.srvs = new List<IntentSrv>();
        }

        private void initContext(ref WBContext context, LUInfo luinfo, string utterance)
        {
            string intent = luinfo.Intent.intent;
            if ((IsLocationOnly(luinfo, utterance))
                || ContainsLocationDateOnly(luinfo, utterance))
            {
                intent = "DefaultIntent";
            }
            
            string location = DatetimeUtils.GetLocation(luinfo, true);
            TimeRange range = DatetimeUtils.GetTimeRange(luinfo);

            context.Intent = intent;
            context.Location = location;
            context.timeRange = range;

            context.validTime = DateTime.Now;
        }

        private void updateContext(ref WBContext context, LUInfo luinfo)
        {
            string intent = luinfo.Intent.intent;

            if (!string.IsNullOrWhiteSpace(intent) && ValidIntent.Contains(intent))
            {
                context.Intent = intent;
            }

            string location = DatetimeUtils.GetLocation(luinfo, false);

            if (!string.IsNullOrWhiteSpace(location))
            {
                context.Location = location;
            }

            TimeRange range = DatetimeUtils.GetTimeRange(luinfo);

            if (range != null)
            {
                context.timeRange = range;
            }
        }

        public string Answer(string userId, string utterance)
        {            
            LUInfo luinfo = this.luController.Understand(utterance);

            WBContext context = contextStore.GetContext(userId);
            
            
            if (!context.IsValid())
            {
                initContext(ref context, luinfo, utterance);
            }
            else
            {
                updateContext(ref context, luinfo);
            }
                        
            switch(context.Intent)
            {
                case "DefaultIntent":
                    this.srvs.Add(new WeatherSrv());
                    this.srvs.Add(new SmogSrv());
                    break;
                case "CarWashing":
                    this.srvs.Add(new CarWashingSrv());
                    break;
                case "Weather":
                    this.srvs.Add(new WeatherSrv());
                    break;
                case "Smog":
                    this.srvs.Add(new SmogSrv());
                    break;
                case "RestrictedDriving":
                    this.srvs.Add(new RestrictedDrivingSrv());
                    break;
                case "Cloth":
                    this.srvs.Add(new ClothSrv());
                    break;
                default:
                    break;                                   
            }

            string answer = "";

            DateTime startTime = DateTime.Now;

            foreach(IntentSrv srv in this.srvs)
            { 
                string singleAnswer = srv.GetAnswer(context, luinfo);
                answer += singleAnswer;
            }

            this.srvs.Clear();

            if (string.IsNullOrWhiteSpace(answer))
            {
                answer = DatetimeUtils.GetOutofScopeAnswer();
            }

            DateTime endTime = DateTime.Now;

            LogUtils.Log(startTime, endTime, "[Seniverse Service]");

            return answer;
        }

        private bool ContainsLocationDateOnly(LUInfo luInfo, string utterance)
        {
            if (luInfo.EntityList == null || luInfo.EntityList.Count == 0)
            {
                return false;
            }

            bool containLocation = false;
            bool containDate = false;
            bool containOther = false;

            string entityStr = "";

            foreach(Entity entity in luInfo.EntityList)
            {
                if (entity.type == "Location")
                {
                    containLocation = true;
                    entityStr += entity.value;
                }
                else if (entity.type == "builtin.datetime.date" || entity.type == "builtin.datetime.time")
                {
                    containDate = true;
                    entityStr += entity.value.Replace(" ", "");
                }
                else
                {
                    containOther = true;
                }
            }


            if (containLocation && containDate && !containOther)
            {
                if (entityStr.Trim().Length == utterance.Trim().Length)
                {
                    return true;
                }
                else if ((((double)entityStr.Trim().Length / (double)utterance.Length) > 0.8) && (luInfo.Intent.intent == "None"))
                { 
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private bool IsLocationOnly(LUInfo luinfo, string utterance)
        {
            string location = DatetimeUtils.GetLocation(luinfo, false);

            if (string.IsNullOrWhiteSpace(location))
            {
                return false;
            }

            if (location == utterance)
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
