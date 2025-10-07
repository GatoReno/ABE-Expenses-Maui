using AbeXP.Common.Enum;
using AbeXP.Resources.Strings;

namespace AbeXP.Extensions
{
    public static class TimePeriodExtensions
    {
        public static string ToLabel(this TimePeriod timePeriod, DateTime date) => timePeriod switch
        {
            TimePeriod.ThreeDays => date.ToString("MMM-dd"),
            TimePeriod.Week => $"{AppResources.Week} {date:MMM-dd}",
            TimePeriod.Month => date.ToString("MMM yyyy"),
            _ => date.ToString("MMM-dd")
        };
    }
}
