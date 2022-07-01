namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using EA.Prsd.Core;

    public static class CurrentSystemTimeHelper
    {
        public static DateTime GetCurrentTimeBasedOnSystemTime(DateTime systemDateTime)
        {
            return new DateTime(systemDateTime.Year, SystemTime.UtcNow.Month, SystemTime.UtcNow.Day, SystemTime.UtcNow.Hour, SystemTime.UtcNow.Minute, SystemTime.UtcNow.Second, SystemTime.UtcNow.Millisecond, SystemTime.UtcNow.Kind);
        }
    }
}
