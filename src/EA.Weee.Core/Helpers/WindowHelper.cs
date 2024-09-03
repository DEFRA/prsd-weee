namespace EA.Weee.Core.Helpers
{
    using System;

    public static class WindowHelper
    {
        public static bool IsThereAnOpenWindow(DateTime systemTime)
        {
            DateTime closedPeriodStart = new DateTime(systemTime.Year, 03, 17);
            DateTime closedPeriodEnd = new DateTime(systemTime.Year, 04, 01);

            return !(systemTime >= closedPeriodStart && systemTime < closedPeriodEnd);
        }

        public static bool IsDateInComplianceYear(int complianceYear, DateTime systemDate)
        {
            if (complianceYear == systemDate.Year)
            {
                return true;
            }

            if (systemDate.Date.Month == 1)
            {
                var previousYear = systemDate.Year - 1;
                if (complianceYear == previousYear)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
