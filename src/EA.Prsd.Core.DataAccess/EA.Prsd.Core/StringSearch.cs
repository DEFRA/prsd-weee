namespace EA.Prsd.Core
{
    using System;

    public class StringSearch
    {
        /// <summary>
        /// Taken from http://en.wikipedia.org/wiki/Levenshtein_distance#Iterative_with_two_matrix_rows
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int CalculateLevenshteinDistance(string s1, string s2)
        {
            if (s1.Equals(s2, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (s1.Length == 0)
            {
                return s2.Length;
            }

            if (s2.Length == 0)
            {
                return s1.Length;
            }

            int[] previousDistances = new int[s2.Length + 1];
            int[] currentDistances = new int[s2.Length + 1];

            // Initialise the previous row of distances.
            // Number of characters to delete from s2.
            for (int i = 0; i < previousDistances.Length; i++)
            {
                previousDistances[i] = i;
            }

            for (int i = 0; i < s1.Length; i++)
            {
                currentDistances[0] = i + 1;

                for (int j = 0; j < s2.Length; j++)
                {
                    int cost = (s1[i] == s2[j]) ? 0 : 1;
                    currentDistances[j + 1] = Math.Min(currentDistances[j] + 1,
                        Math.Min(previousDistances[j + 1] + 1, previousDistances[j] + cost));
                }

                for (int j = 0; j < previousDistances.Length; j++)
                {
                    previousDistances[j] = currentDistances[j];
                }
            }

            return currentDistances[s2.Length];
        }
    }
}
