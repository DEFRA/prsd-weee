namespace EA.Weee.Core.Helpers
{
    using EA.Prsd.Core;
    using System;

    public static class WindowHelper
    {
        public static bool IsWindowClosed()
        {
            DateTime closedPeriodStart = new DateTime(SystemTime.UtcNow.Year, 03, 17);
            DateTime closedPeriodEnd = new DateTime(SystemTime.UtcNow.Year, 04, 01);

            return SystemTime.UtcNow >= closedPeriodStart && SystemTime.UtcNow < closedPeriodEnd;
        }
    }
}
