using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;
using WeatherBot.Engine.Utils;

namespace WeatherBot.Engine.Controller
{
    public abstract class IntentSrv
    {
        

        public abstract string GetAnswer(LUInfo luInfo);

        

    }


}
