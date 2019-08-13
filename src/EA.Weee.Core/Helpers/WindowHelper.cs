namespace EA.Weee.Core.Helpers
{
    using EA.Prsd.Core;
    using System;

    public static class WindowHelper
    {
        public static bool IsThereAnOpenWindow(DateTime systemTime)
        { 
            DateTime closedPeriodStart = new DateTime(systemTime.Year, 03, 17);
            DateTime closedPeriodEnd = new DateTime(systemTime.Year, 04, 01);

            return !(systemTime >= closedPeriodStart && systemTime < closedPeriodEnd);
        }
    }
}
