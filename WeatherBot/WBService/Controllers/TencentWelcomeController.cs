using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;
using WBService.Data;
using WBService.Exceptions;
using WBService.Tencent;
using WBService.Utils;
using WeatherBot.Engine.Controller;
using WeatherBot.Engine.Utils;

namespace WBService.Controllers
{
    public class TencentWelcomeController : ApiController
    {
        private readonly WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(Constants.Token, Constants.EncodingAESKey, Constants.AppId);
        private IntentController bot = new IntentController();

        public HttpResponseMessage Get(string signature = "",  string timestamp="", string nonce="", string echostr = "")
        {            
            string querystr = string.Join("&",
                HttpContext.Current.Request.QueryString
               .AllKeys
               .Select(key => key + "=" + HttpContext.Current.Request.QueryString[key]).ToArray());

            WBService.Utils.Utils.Log("[Get]:\r\n" + querystr);

            int ret = wxcpt.VerifyURL(signature, timestamp, nonce);           

            if (ret != 0)
            {
                WBService.Utils.Utils.Log("Error: Verify failed: " + ret.ToString());
                throw new WebResponseException(HttpStatusCode.InternalServerError, $"VerifyURL failed: {ret}");                
            }
            
            string resp_echostr = echostr;

            HttpResponseMessage resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(resp_echostr, System.Text.Encoding.UTF8, "text/plain") };                      
            
            return resp;
        }

        public HttpResponseMessage Post(HttpRequestMessage request)
        {
           /* var parameters = request.GetQueryNameValuePairs();

            string paramstrs = string.Join("&",
                parameters.ToDictionary(x => x.Key, x => x.Value));               

            Utils.Utils.Log("[Post]:\r\n" + paramstrs);

            string msg_signature = parameters.FirstOrDefault(pair => pair.Key == "signature").Value;
            string timestamp = parameters.FirstOrDefault(pair => pair.Key == "timestamp").Value;
            string nonce = parameters.FirstOrDefault(pair => pair.Key == "nonce").Value;
            if (msg_signature == null || msg_signature.Length == 0 ||
                timestamp == null || timestamp.Length == 0 ||
                nonce == null || nonce.Length == 0)
            {
                throw new WebResponseException(HttpStatusCode.BadRequest, $"Missing necessary params: msg_signature - {msg_signature}, timestamp - {timestamp}, nonce - {nonce}");
            }
            */
            try
            {
                Request req = ParseRequest(request);

                Response resp = GetResponse(req);

                string str_encrypt = EncryptXML(resp);

                Utils.Utils.Log("[Answer]:" + resp.Content);

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(str_encrypt, System.Text.Encoding.UTF8, "text/xml") };

            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        private Request ParseRequest(HttpRequestMessage request)
        {
            string requestXml =  request.Content.ReadAsStringAsync().Result;
            Utils.Utils.Log("[POST input]:\r\n" + requestXml);
            Request req = new Request();

            XmlDocument doc = new XmlDocument();
            XmlNode root;

            try
            {
                doc.LoadXml(requestXml);
                root = doc.FirstChild;
                req.ToUserName = root["ToUserName"].InnerText;
                req.FromUserName = root["FromUserName"].InnerText;
                req.CreateTime = root["CreateTime"].InnerText;
                req.MsgType = root["MsgType"].InnerText;
                req.Content = root["Content"].InnerText;
                req.MsgId = root["MsgId"].InnerText;


                if (req.MsgType == "text")
                {
                    Utils.Utils.Log("[Question]:" + req.Content);
                }
                else
                {
                    Utils.Utils.Log("[Request XML Doc]:\r\n" + requestXml);
                }

                if (req.ToUserName == null || req.MsgId == null || req.Content == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new WebResponseException(HttpStatusCode.BadRequest, $"Missing field: ToUserName - {req.ToUserName}, MsgId - {req.MsgId}, Content - {req.Content}");
            }

            return req;
        }

        private Response GetResponse(Request req)
        {
            string replyMsg = Utils.Utils.GetDefaultAnswer();

            if (req.MsgType == "text")
            {
                DateTime startTime = DateTime.Now;
                replyMsg = Answer(req.Content);
                DateTime endTime = DateTime.Now;

                TimeSpan latency = endTime.Subtract(startTime);
                double ms = latency.TotalMilliseconds;
                Utils.Utils.Log("[Weather Engine Latency] It took " + ms.ToString() + " MS to get answer.");
            }

            Response resp = new Response();
            resp.FromUserName = req.ToUserName;
            resp.ToUserName = req.FromUserName;
            resp.CreateTime = DateTime.Now.Ticks.ToString();
            resp.MsgType = req.MsgType;
            resp.Content = req.MsgType == "text"?replyMsg:"";
            
            return resp;
        }

        private string EncryptXML(Response resp)
        {
            string sEncryptMsg = "";

            string ToUserNameLabelHead = "<ToUserName><![CDATA[";
            string ToUserNameLabelTail = "]]></ToUserName>";

            string FromUserNameLabelHead = "<FromUserName><![CDATA[";
            string FromUserNameLabelTail = "]]></FromUserName>";
            string CreateTimeLabelHead = "<CreateTime><![CDATA[";
            string CreateTimeLabelTail = "]]></CreateTime>";
            string MsgTypeLabelHead = "<MsgType><![CDATA[";
            string MsgTypeLabelTail = "]]></MsgType>";
            string ContentLabelHead = "<Content><![CDATA[";
            string ContentLabelTail = "]]></Content>";

            sEncryptMsg += "<xml>" + ToUserNameLabelHead + resp.ToUserName + ToUserNameLabelTail;
            sEncryptMsg += FromUserNameLabelHead + resp.FromUserName + FromUserNameLabelTail;
            sEncryptMsg += CreateTimeLabelHead + resp.CreateTime + CreateTimeLabelTail;
            sEncryptMsg += MsgTypeLabelHead + resp.MsgType + MsgTypeLabelTail;
            sEncryptMsg += ContentLabelHead + resp.Content + ContentLabelTail;
            sEncryptMsg += "</xml>";

            Utils.Utils.Log("[POST output]:\r\n" + sEncryptMsg);
            return sEncryptMsg;
        }

        private string Answer(string utterance)
        {
            string answer = this.bot.Answer(utterance);

            if (string.IsNullOrWhiteSpace(answer) || answer.Contains(LuisUtils.GetOutofScopeAnswer()))
            {
                answer = Utils.Utils.GetDefaultAnswer();
            }

            return answer;
        }

    }

    public class Record
    {
        public string msg_signature { get; set; }
        public string timestamp { get; set; }
        public string nonce { get; set; }        
        public string echostr { get; set; }
        public int ret { get; set; }
        public string echostr_decrypt { get; set; }
    }

    public class Constants
    {
        public static string Token = /*Convert.ToBase64String(Encoding.ASCII.GetBytes(*/"20161222m75ddxxxy01abc"/*))*/; 
        public static string EncodingAESKey = "YqVyCcqrNiOBUul6rESTyu6fazzD3caGYy0Fd1gDBSb";
        public static string AppId = "wx3c10f06ab420e457";
    }

    
}