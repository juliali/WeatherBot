using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBot.Engine.Data;

namespace WeatherBot.Engine.Utils
{
    public class DatetimeUtils
    {
        public static DatePeriod ConvertDateArea(List<string> dateareas)
        {
            DateTime today = DateTime.Now.Date;
            if (dateareas == null || dateareas.Count == 0)
            {
                return null;
            }

            if (dateareas.Count == 1)
            {
                string datearea = GetExactDateArea(today, null, dateareas[0]);                
                return ConvertSingleDateArea(today, datearea);
            }
            else
            {
                List<DatePeriod> datePeriods = new List<DatePeriod>();
                for (int i = 0; i < dateareas.Count; i ++)
                {
                    string datearea;
                    if (i == 0)
                    {
                        datearea = GetExactDateArea(today, null, dateareas[i]);
                    }
                    else
                    {
                        datearea = GetExactDateArea(today, dateareas[i - 1], dateareas[i]);
                    }
                    
                    datePeriods.Add(ConvertSingleDateArea(today, datearea));
                    dateareas[i] = datearea;
                }
                DatePeriod datePeriod = new DatePeriod();
                datePeriod.startDateStr = datePeriods[0].startDateStr;
                datePeriod.endDateStr = datePeriods[datePeriods.Count - 1].endDateStr;

                return datePeriod;                
            }
        }

        private static string GetExactDateArea(DateTime today, string previousDateare, string currentDatearea)
        {           
            if (currentDatearea.StartsWith("XXXX-WXX-"))
            {
                if (string.IsNullOrWhiteSpace(previousDateare))
                {

                    int weekdays = int.Parse(currentDatearea.Replace("XXXX-WXX-", ""));
                    DateTime date = today.AddDays(weekdays - GetTodayWeekDay(today));
                    string datearea = date.ToString("yyyy-MM-dd");
                    return datearea;

                }
                else
                {
                    string rule = "^[0-9]{4}-W[0-9]{2}-";

                    List<string> matchedValues = FileUtils.Match(rule, previousDateare);
                    if (matchedValues.Count > 0)
                    {
                        string datearea = currentDatearea.Replace("XXXX-WXX-", matchedValues[0]);
                        return datearea;
                    }
                    else
                    {
                        string rule2 = "^[0-9]{4}-[0-9]{2}-[0-9]{2}$";
                        matchedValues = FileUtils.Match(rule2, previousDateare);

                        if (matchedValues.Count > 0)
                        {
                            DateTime previousDate = DateTime.Parse(previousDateare);
                            int dwOfPrevious = (int) previousDate.DayOfWeek;
                            if (dwOfPrevious == 0)
                            {
                                dwOfPrevious = 7;
                            }

                            try
                            { 
                                int dwOfCurrent = int.Parse(currentDatearea.Replace("XXXX-WXX-", ""));
                                DateTime date = previousDate.AddDays(dwOfCurrent - dwOfPrevious);

                                return date.ToString("yyyy-MM-dd");
                            }
                            catch(Exception)
                            {
                                return currentDatearea;
                            }
                        }
                        else
                        { 
                            return currentDatearea; ///TODO
                        }
                    }
                }
            }
            else
            {
                return currentDatearea;
            }
        }

        private static int GetTodayWeekDay(DateTime today)
        {
            switch(today.DayOfWeek)
            { 
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
                default: return 7;
            }
        }
        private static DatePeriod ConvertSingleDateArea(DateTime today, string datearea)
        {            
            if (string.IsNullOrWhiteSpace(datearea))
            {
                return null;
            }

            try
            { 
                DateTime dateTime = DateTime.Parse(datearea);
                string datestr = dateTime.ToString("yyyy-MM-dd");

                DatePeriod datePeriod = new DatePeriod();
                datePeriod.startDateStr = datestr;
                datePeriod.endDateStr = datestr;

                return datePeriod;
            }
            catch (Exception e)
            {
                //2016 - W52 - WE
                string[] tmps = datearea.Split('-');

                if (tmps.Length < 2 || tmps.Length > 3)
                {
                    return null;
                }
                    
                try
                {
                    string rule1 = "^XXXX-XX-[0-9]{2}";

                    List<string> matchedValues = FileUtils.Match(rule1, datearea);

                    if (matchedValues.Count > 0)
                    {
                        bool IsThisMonth = true;
                        try
                        { 
                            int dayNum = int.Parse(datearea.Replace("XXXX-XX-", ""));
                            if (dayNum < today.Day)
                            {
                                IsThisMonth = false;
                            }
                        }
                        catch(Exception)
                        {

                        }
                        int yearNum = today.Year;
                        int monthNum = today.Month;

                        if (!IsThisMonth)
                        {
                            monthNum += 1;

                            if (monthNum > 12)
                            {
                                monthNum -= 12;
                                yearNum += 1;
                            }
                        }

                        string yearmonthStr = yearNum.ToString() + "-" + monthNum.ToString();
                        string newdatearea = datearea.Replace("XXXX-XX", yearmonthStr);

                        return ConvertSingleDateArea(today, newdatearea);
                    }
                    else
                    {
                        string rule2 = "^XXXX-[0-9]{2}-[0-9]{2}";
                        matchedValues = FileUtils.Match(rule2, datearea);

                        if (matchedValues.Count > 0)
                        {
                            bool IsThisMonth = true;
                            string[] tmpStrs = datearea.Split('-');
                            try
                            {
                                int monthNum = int.Parse(tmpStrs[1]);

                                if (monthNum < today.Month)
                                {
                                    IsThisMonth = false;
                                }
                            }
                            catch (Exception)
                            {

                            }

                            int yearNum = today.Year;

                            if (!IsThisMonth)
                            {
                                yearNum += 1;
                            }

                            string yearStr = yearNum.ToString();
                            string newdatearea = datearea.Replace("XXXX", yearStr);
                            return ConvertSingleDateArea(today, newdatearea);
                        }
                        else
                        {
                            int year = int.Parse(tmps[0]);
                            int week = int.Parse(tmps[1].Replace("W", ""));
                            string days = "";

                            if (tmps.Length == 3)
                            {
                                days = tmps[2].Replace("W", "");
                            }

                            List<DayOfWeek> dayofweeklist = new List<DayOfWeek>();

                            if (days == "")
                            {
                                dayofweeklist.Add(DayOfWeek.Monday);
                                dayofweeklist.Add(DayOfWeek.Tuesday);
                                dayofweeklist.Add(DayOfWeek.Wednesday);
                                dayofweeklist.Add(DayOfWeek.Tuesday);
                                dayofweeklist.Add(DayOfWeek.Friday);
                                dayofweeklist.Add(DayOfWeek.Saturday);
                                dayofweeklist.Add(DayOfWeek.Sunday);
                            }
                            else if (days == "E")
                            {
                                dayofweeklist.Add(DayOfWeek.Saturday);
                                dayofweeklist.Add(DayOfWeek.Sunday);
                            }
                            else
                            {
                                try
                                {
                                    int dayNum = int.Parse(tmps[2]);
                                    switch (dayNum)
                                    {
                                        case 1: dayofweeklist.Add(DayOfWeek.Monday); break;
                                        case 2: dayofweeklist.Add(DayOfWeek.Tuesday); break;
                                        case 3: dayofweeklist.Add(DayOfWeek.Wednesday); break;
                                        case 4: dayofweeklist.Add(DayOfWeek.Thursday); break;
                                        case 5: dayofweeklist.Add(DayOfWeek.Friday); break;
                                        case 6: dayofweeklist.Add(DayOfWeek.Saturday); break;
                                        case 7: dayofweeklist.Add(DayOfWeek.Sunday); break;
                                    }
                                }
                                catch (Exception)
                                {
                                    return null;
                                }
                            }

                            DateTime janfirst = new DateTime(year, 1, 1);
                            DayOfWeek firstdayofweek = janfirst.DayOfWeek;

                            int offset = firstdayofweek - DayOfWeek.Monday;

                            DateTime date = janfirst.AddDays((week - 1) * 7 + offset);

                            int startOffset = dayofweeklist[0] - date.DayOfWeek;
                            DateTime start = date.AddDays(startOffset);

                            int endOffset = dayofweeklist[dayofweeklist.Count - 1] - date.DayOfWeek;

                            if (endOffset < 0)
                            {
                                endOffset += 7;
                            }
                            DateTime end = date.AddDays(endOffset);

                            DatePeriod dateperiod = new DatePeriod();
                            dateperiod.startDateStr = start.ToString("yyyy-MM-dd");
                            dateperiod.endDateStr = end.ToString("yyyy-MM-dd");

                            return dateperiod;
                        }
                    }
                }
                catch(Exception e2)
                {
                    return null;
                }

            }
        }

        public static TimePeriod ConvertTimeArea(List<string> timeareas)
        {
            if (timeareas == null || timeareas.Count == 0)
            {
                return null;
            }

            if (timeareas.Count == 1)
            {
                return ConvertSingleTimeArea(timeareas[0]);
            }
            else
            {
                string startStr = timeareas[0];
                string endStr = timeareas[timeareas.Count - 1];

                if (startStr.StartsWith("T") && endStr.StartsWith("T"))
                {
                    
                    try
                    { 
                        int startInt = int.Parse(startStr.Replace("T", ""));
                        int endInt = int.Parse(endStr.Replace("T", ""));

                        if (startInt > 12 && endInt < startInt)
                        {
                            endInt += 12;
                        }
                        endStr = "T" + endInt.ToString();
                    }
                    catch(Exception)
                    {

                    }
                }

                TimePeriod start = ConvertSingleTimeArea(startStr);
                TimePeriod end = ConvertSingleTimeArea(endStr);

                TimePeriod result = new TimePeriod();
                result.startHour = start.startHour;
                result.endHour = end.endHour;

                return result;
            }
        }

        private static TimePeriod ConvertSingleTimeArea(string timearea)
        {
            if (string.IsNullOrWhiteSpace(timearea))
            {
                return null;
            }

            string timeRule = "^T[0-9]{2}$";

            List<String> matchedStrs = FileUtils.Match(timeRule, timearea);

            TimePeriod timePeriod = new TimePeriod();

            if (matchedStrs.Count > 0)
            {
                string matchedStr = matchedStrs[0];
                matchedStr = matchedStr.Replace("T", "");

                try
                { 
                    int hour = int.Parse(matchedStr);
                    timePeriod.startHour = hour;
                    timePeriod.endHour = hour;
                }
                catch(Exception e)
                {
                    timePeriod = null;
                }

            }
            else
            {                
                if (timearea == "TMO")
                {
                    timePeriod.startHour = 6;
                    timePeriod.endHour = 12;

                }
                else if (timearea == "TAF")
                {
                    timePeriod.startHour = 12;
                    timePeriod.endHour = 18;
                }
                else if (timearea == "TNI")
                {
                    timePeriod.startHour = 18;
                    timePeriod.endHour = 24;
                }
                else
                {
                    timePeriod = null;
                }               
            }
            return timePeriod;
        }

        public static string GetOutofScopeAnswer()
        {
            return "这个问题暂时无法回答";
        }

        public const string DefaultLocation = "北京";

        public static string GetLocation(LUInfo luInfo, bool useDefault)
        {
            List<string> provices = GetEntity(luInfo, "Province");
            List<string> locations = GetEntity(luInfo, "Location");

            if (locations == null || locations.Count == 0)
            {
                if (useDefault)
                { 
                    return DefaultLocation;
                }
                else
                {
                    return null;
                }
            }

            if (provices != null && provices.Count > 0)
            {
                provices.AddRange(locations);
                locations = provices;
            }
            return string.Join("", locations);//locations[locations.Count - 1];
        }

        public static DatePeriod GetDatePeriod(LUInfo luInfo)
        {
            List<string> dateareas = GetEntity(luInfo, "builtin.datetime.date");

            return DatetimeUtils.ConvertDateArea(dateareas);
        }

        public static TimePeriod GetTimePeriod(LUInfo luInfo)
        {
            List<string> timeareas = GetEntity(luInfo, "builtin.datetime.time");

            return DatetimeUtils.ConvertTimeArea(timeareas);
        }

        public static List<string> GetEntity(LUInfo luInfo, string EntityType)
        {
            List<string> result = new List<string>();

            if (luInfo.EntityList.Count != 0)
            {
                foreach (Entity entity in luInfo.EntityList)
                {
                    if (entity.type == EntityType)
                    {
                        if (string.IsNullOrWhiteSpace(entity.resolution))
                        {
                            result.Add(entity.value);
                        }
                        else
                        {
                            result.Add(entity.resolution);
                        }
                    }
                }
            }

            return result;
        }

        public static TimeRange GetTimeRange(LUInfo luInfo)
        {
            DatePeriod datePeriod = GetDatePeriod(luInfo);
            TimePeriod timePeriod = GetTimePeriod(luInfo);

            if (datePeriod == null && timePeriod == null)
            {
                return null;
            }
            else
            {
                TimeRange range = new TimeRange();

                DateTime startDate;
                DateTime endDate;

                if (timePeriod == null)
                {
                    startDate = DateTime.Parse(datePeriod.startDateStr);
                    endDate = DateTime.Parse(datePeriod.endDateStr);
                }
                else
                {
                    DateTime today = DateTime.Now.Date;

                    startDate = today;
                    endDate = today;

                    if (datePeriod != null)
                    {
                        startDate = DateTime.Parse(datePeriod.startDateStr);
                        endDate = DateTime.Parse(datePeriod.endDateStr);
                    }

                    startDate = startDate.AddHours(timePeriod.startHour);
                    endDate = endDate.AddHours(timePeriod.endHour);

                    range.IsHourly = true;

                }

                range.startDate = startDate;
                range.endDate = endDate;

                return range;
            }
        }
    }

    public class TimeRange
    {
        public DateTime startDate;
        public DateTime endDate;

        public bool IsHourly = false;
    }
}
