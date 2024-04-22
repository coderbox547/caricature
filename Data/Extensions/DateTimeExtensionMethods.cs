using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Extensions
{
    public static class DateTimeExtensionMethods
    {
        public static DateTime GetIndianCurrentDate(this DateTime src)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time"); 
        }


        public static DateTime GetDateOnTimezoneBase(this DateTime src)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(src, "India Standard Time");
            //return src;
        }

        public static DateTime SetMinTime(this DateTime src)
        {
            return new System.DateTime(src.Year, src.Month, src.Day, 0, 0, 0);
        }

        public static DateTime SetMaxTime(this DateTime src)
        {
            return new System.DateTime(src.Year, src.Month, src.Day, 23, 59, 59);
        }

        public static string ConvertDateTimeAsString(this DateTime src)
        {
            return src.ToString("dd/MM/yyyy hh:mm:ss tt");//.ToShortDateString();
        }
        public static string ConvertDateTimeAsStringFormatted(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }
        public static string ConvertDateAsString(this DateTime src)
        {
            return src.ToString("dd/MM/yy");//.ToShortDateString();
        }

        public static string ConvertTimeAsString(this DateTime src)
        {
            return src.ToString("hh:mm:ss");//.ToShortDateString();
        }

        public static string ConvertTimeAsString(this TimeSpan src)
        {
            return src.ToString("hh\\:mm\\:ss");//.ToShortDateString();
        }      

        public static string DateTimeAsHumanableText(this DateTime from, DateTime to)
        {
            string retVal = string.Empty;

            int totalDays = Convert.ToInt32((from - to).TotalDays);

            switch (totalDays)
            {
                case 0:
                    retVal = ConvertTimeAsString(to);
                    break;

                case 1:
                    retVal = "yesterday";
                    break;

                default:
                    retVal = string.Concat(totalDays.ToString(), " days");
                    break;
            }

            return retVal;
        }

        public static int DateDiff(this DateTime from, DateTime to)
        {

            int totalDays = Convert.ToInt32((from - to).TotalDays);

            return totalDays;
        }

        public static string ConvertDateTimeToGMT10DateTimeString(this DateTime src)
        {
            return TimeZoneInfo.ConvertTime(src, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time")).ToString("dd/MM/yyyy hh:mm:ss");
        }

        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        public static double GetDaysBetweenDates(this DateTime endDate, DateTime startDate)
        {
            return (endDate - startDate).TotalDays;
        }

        public static DateTime GetLastDayOfMonth(this DateTime src, int? month = null, int? year = null)
        {
            DateTime today = DateTime.Today;

            if (!month.HasValue)
                month = today.Month;

            if (!year.HasValue)
                year = today.Year;

            int day = DateTime.DaysInMonth(year.Value, month.Value);
            DateTime retVal = new DateTime(year.Value, month.Value, day);
            return retVal;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime src, int? month = null, int? year = null)
        {
            DateTime today = DateTime.Today;

            if (!month.HasValue)
                month = today.Month;

            if (!year.HasValue)
                year = today.Year;

            DateTime retVal = new DateTime(year.Value, month.Value, 1);
            return retVal;
        }
        public static int GetWeeksInYear(this DateTime src, int year)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = new DateTime(year, 12, 31);
            Calendar cal = dfi.Calendar;
            return cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        public static List<WeekDay> GetFirstAndLastDateOfWeek(this DateTime src, int year)
        {
            List<WeekDay> retVal = new List<WeekDay>();

            int numberOfWeeksInYear = default(DateTime).GetIndianCurrentDate().GetWeeksInYear(year);
            DateTime createdDateFrom = default(DateTime);
            DateTime createdDateTo = default(DateTime);
            for (int weekIndex = 0; weekIndex <= numberOfWeeksInYear; weekIndex++)
            {
                createdDateFrom = default(DateTime).GetIndianCurrentDate().FirstDateOfWeekISO8601(year, weekIndex);
                createdDateTo = createdDateFrom.AddDays(6);

                if (createdDateFrom.Year != year + 1 && createdDateTo.Year >= year)
                {
                    // If value of createdDateFrom is lie in preview year then replace with first day of the current year
                    if (year > createdDateFrom.Year)
                        createdDateFrom = new DateTime(year, 1, 1);

                    // If value of createdDateTo is lie in next year then replace with last day of the current year
                    if (createdDateTo.Year > year)
                        createdDateTo = new DateTime(year, 12, 31);

                    retVal.Add(new WeekDay
                    {
                        FirstDateOfWeek = createdDateFrom,
                        LastDateOfWeek = createdDateTo
                    });

                }
            }


            return retVal;
        }

        public static DateTime FirstDateOfWeekISO8601(this DateTime src, int year, int weekOfYear)
        {
            //int year = src.Year;

            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        public static DateTime ConvertDateTimeToGMT10DateTime(this DateTime src)
        {
            return TimeZoneInfo.ConvertTime(src, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));
        }
        public static DateTime ConvertGMT10DateTimeToUtcTime(this DateTime src)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(src, DateTimeKind.Unspecified), tz);
        }
        public static string GetTimeBetweenDates(this DateTime endDate, DateTime startDate)
        {
            TimeSpan _date = (endDate - startDate);
            return string.Format("{0}:{1}:{2}", Math.Round(_date.TotalHours), Math.Round(_date.TotalMinutes), Math.Round(_date.TotalSeconds));
        }


    }



    public class WeekDay
    {
        public DateTime FirstDateOfWeek { get; set; }
        public DateTime LastDateOfWeek { get; set; }
    }
}
