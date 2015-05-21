namespace EA.Prsd.Core
{
    using System;

    /// <summary>
    /// A testable alternative to DateTime.Now
    /// </summary>
    /// <remarks>
    /// https://github.com/rbwestmoreland/system.clock
    /// </remarks>
    public static class SystemTime
    {
        private static Func<DateTime> utcNowInstance = () => DateTime.UtcNow;

        /// <summary>
        /// Equivalent to DateTime.UtcNow
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                return utcNowInstance();
            }
        }

        /// <summary>
        /// Equivalent to DateTime.Now
        /// </summary>
        public static DateTime Now 
        { 
            get 
            { 
                return utcNowInstance().ToLocalTime(); 
            } 
        }

        /// <summary>
        /// Freeze time
        /// </summary>
        public static void Freeze()
        {
            var instance = new DateTime(DateTime.UtcNow.Ticks);
            utcNowInstance = () => instance;
        }

        /// <summary>
        /// Freeze at a specific time
        /// </summary>
        /// <param name="dateTime">a datetime</param>
        /// <param name="isLocal">true if local, false if utc</param>
        public static void Freeze(DateTime dateTime, bool isLocal = false)
        {
            var instance = isLocal ? new DateTime(dateTime.Ticks).ToUniversalTime() : new DateTime(dateTime.Ticks);
            utcNowInstance = () => instance;
        }

        /// <summary>
        /// Unfreeze time and return to the present
        /// </summary>
        public static void Unfreeze()
        {
            utcNowInstance = () => DateTime.UtcNow;
        }
    }
}