namespace EA.Weee.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ExtensionMethods
    {
        public static bool ElementsEqual<T>(this ICollection<T> first, ICollection<T> second)
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

                result = first.SequenceEqual(secondList);
            }

            return result;
        }
    }
}
