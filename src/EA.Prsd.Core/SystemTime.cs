namespace EA.Prsd.Core
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A testable alternative to DateTime.Now
    /// </summary>
    /// <remarks>
    /// https://github.com/rbwestmoreland/system.clock
    /// </remarks>
    public static class SystemTime
    {
        private static AmbientSingleton<Func<DateTime>> utcNowInstance;

        static SystemTime()
        {
            utcNowInstance = new AmbientSingleton<Func<DateTime>>(() => DefaultUtcNow());
        }

        /// <summary>
        /// Equivalent to DateTime.UtcNow
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                return utcNowInstance.Value();
            }
        }

        /// <summary>
        /// Equivalent to DateTime.Now
        /// </summary>
        public static DateTime Now 
        { 
            get 
            {
                var now = utcNowInstance.Value().ToLocalTime();
                now = DateTime.SpecifyKind(now, DateTimeKind.Local);
                return now;
            } 
        }

        /// <summary>
        /// Freeze time
        /// </summary>
        public static void Freeze()
        {
            var instance = new DateTime(DateTime.UtcNow.Ticks);
            instance = DateTime.SpecifyKind(instance, DateTimeKind.Utc);
            utcNowInstance.Value = () => instance;
        }

        /// <summary>
        /// Freeze at a specific time
        /// </summary>
        /// <param name="dateTime">a datetime</param>
        /// <param name="isLocal">true if local, false if utc</param>
        public static void Freeze(DateTime dateTime, bool isLocal = false)
        {
            var instance = isLocal ? new DateTime(dateTime.Ticks).ToUniversalTime() : new DateTime(dateTime.Ticks);
            instance = DateTime.SpecifyKind(instance, isLocal ? DateTimeKind.Local : DateTimeKind.Utc);
            utcNowInstance.Value = () => instance;
        }

        /// <summary>
        /// Unfreeze time and return to the present
        /// </summary>
        public static void Unfreeze()
        {
            utcNowInstance.Value = DefaultUtcNow;
        }

        /// <summary>
        /// Convert the year, month and day to a System.DateTime, and returns a value that indicates
        /// whether the conversion succeeded.
        /// </summary>
        /// <param name="year">The year (1 through 9999)</param>
        /// <param name="month">The month (1 through 12)</param>
        /// <param name="day">The day (1 through number of days in <b>month</b>)</param>
        /// <param name="result">When this method returns, contains the DateTime equivalent
        /// of the given year, month and day if the conversion succeeded, or DateTime.MinValue
        /// if the conversion failed.</param>
        /// <returns></returns>
        public static bool TryParse(int year, int month, int day, out DateTime result)
        {
            var dateString = string.Format("{0}{1}{2}", year.ToString("D4"), month.ToString("D2"), day.ToString("D2"));

            return DateTime.TryParseExact(dateString, "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        private static DateTime DefaultUtcNow()
        {
            var utcNow = DateTime.UtcNow;
            utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
            return utcNow;
        }
    }
}