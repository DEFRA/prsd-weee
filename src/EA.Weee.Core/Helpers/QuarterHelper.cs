namespace EA.Weee.Core.Helpers
{
    using System;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;

    public static class QuarterHelper
    {
        public static bool IsOpenForReporting(QuarterWindow quarterWindow)
        {
            var reportingStartDate = quarterWindow.StartDate.AddMonths(3);
            var reportingEndDate = new DateTime(quarterWindow.StartDate.Year + 1, 03, 16);

            return reportingStartDate <= SystemTime.UtcNow && reportingEndDate >= SystemTime.UtcNow;
        }
    }
}