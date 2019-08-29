namespace EA.Prsd.Core.Identity
{
    using System;

    /// <summary>
    ///     An <see cref="IIdentityGenerator" /> that generates <see cref="System.Guid" /> values
    ///     using a strategy suggested Jimmy Nilsson's
    ///     <a href="http://www.informit.com/articles/article.asp?p=25862">article</a>
    ///     on <a href="http://www.informit.com">informit.com</a>.
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         The <c>comb</c> algorithm is designed to make the use of GUIDs as Primary Keys, Foreign Keys,
    ///         and Indexes nearly as efficient as ints.
    ///     </p>
    ///     <p>
    ///         This code was taken from
    ///         <a
    ///             href="https://raw.githubusercontent.com/nhibernate/nhibernate-core/master/src/NHibernate/Id/GuidCombGenerator.cs">
    ///             NHibernate
    ///         </a>
    ///         .
    ///     </p>
    /// </remarks>
    public static class GuidCombGenerator
    {
        /// <summary>
        ///     Generate a new <see cref="Guid" /> using the comb algorithm.
        /// </summary>
        public static Guid GenerateComb()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}