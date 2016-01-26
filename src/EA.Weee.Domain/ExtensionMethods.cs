namespace EA.Weee.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExtensionMethods
    {
        public static bool ElementsEqual<T>(this ICollection<T> first, ICollection<T> second)
            where T : IComparable<T>
        {
            bool result;

            if (ReferenceEquals(first, second))
            {
                result = true;
            }
            else if (first == null ||
                     second == null ||
                     first.Count != second.Count)
            {
                result = false;
            }
            else
            {
                var firstList = first.ToList();
                var secondList = second.ToList();

                firstList.Sort();
                secondList.Sort();

                result = firstList.SequenceEqual(secondList);
            }

            return result;
        }

        /// <summary>
        /// Compares two collections to determine whether they contain the same items. The order in which
        /// the items appear in the collection is not relevant. The items have to implement <see cref="IEquatable{T}"/>
        /// but do not have to implement <see cref="IComparable{T}"/>
        /// </summary>
        /// <typeparam name="T">Any implementation of <see cref="IEquatable{T}"/></typeparam>
        /// <param name="first">The first collection</param>
        /// <param name="second">The second collection.</param>
        /// <returns>True if the collections contain the same items, false otherwise.</returns>
        public static bool UnorderedEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
            where T : IEquatable<T>
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            else if (first == null ||
                     second == null ||
                     first.Count() != second.Count())
            {
                return false;
            }
            else
            {
                return !first.Except(second).Any() &&
                       !second.Except(first).Any();
            }
        }
    }
}
