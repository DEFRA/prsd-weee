namespace EA.Prsd.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        /// <summary>
        ///     Returns an IEnumerable of the collection that can't be cast back to ICollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToSafeIEnumerable<T>(this ICollection<T> collection)
        {
            return collection == null ? new T[] { } : collection.Skip(0);
        }
    }
}