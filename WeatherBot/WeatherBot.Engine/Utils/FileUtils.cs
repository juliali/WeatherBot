using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using WeatherBot.Engine.Data;

namespace WeatherBot.Engine.Utils
{
    public class FileUtils
    {
        public static string ReadEmbeddedResourceFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public static List<PreprocessRuleInfo> ReadPreprocessInfo(string ResourceName)
        {
            List<PreprocessRuleInfo> results = new List<PreprocessRuleInfo>();

            string content = ReadEmbeddedResourceFile(ResourceName);
            string[] lines = content.Split("\r\n".ToCharArray());

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] tmps = line.Split('\t');

                if (tmps.Length == 3)
                {
                    PreprocessRuleInfo info = new PreprocessRuleInfo();
                    info.ValueRegex = tmps[0];
                    info.OrigValue = tmps[1];
                    info.NewValue = tmps[2];
                   
                    results.Add(info);
                }
            }

            return results;
        }

        public static List<ExtractorRuleInfo> ReadExtractorInfo(string ResourceName)
        {
            List<ExtractorRuleInfo> results = new List<ExtractorRuleInfo>();

            string content = ReadEmbeddedResourceFile(ResourceName);
            string[] lines = content.Split("\r\n".ToCharArray());

            foreach(string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] tmps = line.Split('\t');

                if (tmps.Length >= 2)
                {
                    ExtractorRuleInfo info = new ExtractorRuleInfo();
                    info.ValueRegex = tmps[0];
                    info.Type = tmps[1];

                    if (tmps.Length > 2)
                    {
                        info.ResolutionValue = tmps[2];
                    }

                    results.Add(info);
                }
            }

            return results;
        }

        public static List<string> Match(string Rule, string text)
        {
            List<string> results = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return results;
            }

            text = text.Trim();

            Regex r = new Regex(Rule, RegexOptions.IgnoreCase);

            Match m = r.Match(text);

            while (m.Success)
            {

                results.Add(m.Value);
                m = m.NextMatch();
            }

            return results;
        }

        public static string ReplaceRegx(PreprocessRuleInfo info, string text)
        {
            if(string.IsNullOrWhiteSpace(text) || info == null)
            {
                return text;
            }

            string result = text;

            List <string> matchedValues = Match(info.ValueRegex, text);

            foreach(string machtedValue in matchedValues)
            { 
                result = Regex.Replace(text, info.OrigValue, info.NewValue, RegexOptions.IgnoreCase);
            }

            return result;
        }

        public static DateOffsetInfo GetDateOffset(DateTime startDate, DateTime endDate)
        {
            DateTime today = DateTime.Now.Date;

            int start = startDate.Subtract(today).Days;
            int end = endDate.Subtract(today).Days;

            int len = end - start + 1;

            DateOffsetInfo dateOffset = new DateOffsetInfo();
            dateOffset.start = start;
            dateOffset.len = len;

            return dateOffset;
        }

        public static DateOffsetInfo GetTimeOffset(DateTime startDate, DateTime endDate)
        {
            // DateTime nowDate = DateTime.Now.ToLocalTime();
            DateTime nowDate = TimeZoneInfo.ConvertTime(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

            TimeSpan startSpan = startDate.Subtract(nowDate);
            TimeSpan endSpan = endDate.Subtract(nowDate);

            int start = (int) startSpan.TotalHours;

            if (startSpan.TotalHours > (double) start)
            {
                start += 1;
            }

            int end = (int)endSpan.TotalHours;

            if (endSpan.TotalHours > (double) end)
            {
                end += 1;
            }

            int len = end - start;

            DateOffsetInfo dateOffset = new DateOffsetInfo();
            dateOffset.start = start;
            dateOffset.len = len;

            return dateOffset;
        }
    }
}
