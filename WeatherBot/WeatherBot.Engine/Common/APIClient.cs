using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBot.Engine.Common
{
    public class APIClient
    {
        public string Query(string completeURL)
        {
            //创建post请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(completeURL);
            request.Method = "GET";

            //接受返回来的数据
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string value = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            response.Close();

            Console.WriteLine(value);
            Console.WriteLine("");

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                string resp = value.ToString();
                return resp;
            }

        }
    }
}
