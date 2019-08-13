namespace EA.Weee.Web.Infrastructure
{
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;

    public static class DisplayHelper
    {
        public static string ReportingOnValue(string name, string number)
        {
            string value = string.Empty;
           
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(number))
            {
                return value + name + " " + "(" + number + ")";
            }
            return value;
        }

        public static string YearQuarterPeriodFormat(Quarter quarter, QuarterWindow quarterWindow)
        {
            return quarter != null & quarterWindow != null ? $"{string.Concat(quarter.Year, " ", quarter.Q)} {quarterWindow.QuarterStart.ToString("MMM", CultureInfo.CurrentCulture)} - {quarterWindow.QuarterEnd.ToString("MMM", CultureInfo.CurrentCulture)}" : string.Empty;
        }

        public static string QuarterPeriodFormat(string quarter, QuarterWindow quarterWindow)
        {
            return
                $"{quarter} {quarterWindow.QuarterStart.ToString("MMM", CultureInfo.CurrentCulture)} - {quarterWindow.QuarterEnd.ToString("MMM", CultureInfo.CurrentCulture)}";
        }
    }
}