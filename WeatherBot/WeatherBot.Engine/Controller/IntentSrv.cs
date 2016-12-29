using WeatherBot.Engine.Data;


namespace WeatherBot.Engine.Controller
{
    public abstract class IntentSrv
    {        
        public abstract string GetAnswer(WBContext context, LUInfo luInfo);
    }
}
