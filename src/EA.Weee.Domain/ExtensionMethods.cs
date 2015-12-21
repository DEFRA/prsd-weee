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

            if (object.ReferenceEquals(first, second))
            {
                result = true;
            }
            else if (first == null || second == null)
            {
                result = false;
            }
            else if (first.Count != second.Count)
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
    }
}
