using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Extensions
{
    internal static class DateTimeExtensions
    {
        public static DateTime GetPeriodStart(this DateTime date, TimePeriod period)
        {
            return period switch
            {
                TimePeriod.ThreeDays => date.AddDays(-(date.Day % 3)).Date, // start of 3-day block
                TimePeriod.Week => date.AddDays(-(int)date.DayOfWeek).Date,   // start of the week (Sunday)
                TimePeriod.Month => new DateTime(date.Year, date.Month, 1).Date, // first day of month
                _ => date
            };
        }

        public static DateTime FirstDayOfCurrentMonth(this DateTime now)
        {
            return new DateTime(now.Year, now.Month, 1);
        }

        public static DateTime LastDayOfCurrentMonth(this DateTime now)
        {
            return new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        }
    }
}
