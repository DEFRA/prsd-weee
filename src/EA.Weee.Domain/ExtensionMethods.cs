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

        public static bool UnorderedEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
            where T : IEquatable<T>
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            else if (a == null ||
                     b == null ||
                     a.Count() != b.Count())
            {
                return false;
            }
            else
            {
                return !a.Except(b).Any() &&
                       !b.Except(a).Any();
            }
        }
    }
}
