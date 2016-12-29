using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.LUEngine.Luis;
using WeatherBot.Engine.LUEngines;
using WeatherBot.Engine.LUEngines.RulebasedEngine;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public class IntentController
    {
        private IntentSrv srv;
        private LUController luController = new LUController();
       

        public IntentController()
        {

        }

        private string GetDefaultIntentResponse(LUInfo luinfo)
        {
            WeatherSrv weatherSrv = new WeatherSrv();
            SmogSrv smogSrv = new SmogSrv();

            string resp = weatherSrv.GetAnswer(luinfo);
            resp += "\r\n" + smogSrv.GetAnswer(luinfo);

            return resp;
        }

        public string Answer(string utterance)
        {            
            LUInfo luinfo = this.luController.Understand(utterance);

            string location = LuisUtils.GetLocation(luinfo);

            if ((IsLocationOnly(location, utterance))
                || ContainsLocationDateOnly(luinfo, utterance))
            {
                return GetDefaultIntentResponse(luinfo);
            }

            switch(luinfo.Intent.intent)
            {
                case "CarWashing":
                    this.srv = new CarWashingSrv();
                    break;
                case "Weather": 
                    this.srv = new WeatherSrv();
                    break;
                case "Smog":
                    this.srv = new SmogSrv();
                    break;
                case "RestrictedDriving":
                    this.srv = new RestrictedDrivingSrv();
                    break;
                case "Cloth":
                    this.srv = new ClothSrv();
                    break;
                default:                     
                    return LuisUtils.GetOutofScopeAnswer();                    
            }

            string answer = this.srv.GetAnswer(luinfo);

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

        private bool IsLocationOnly(string location, string utterance)
        {
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
