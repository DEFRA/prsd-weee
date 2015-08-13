namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System;

    public static class RandomHelper
    {
        public static bool OneIn(int x)
        {
            return R.Next(x) == 0;
        }

        public static T ChooseEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(R.Next(values.Length));
        }

        /// <summary>
        /// Generates a random string of characters, optionally starting with a specific prefix.
        /// A quadratic distribution will be used to determine the string length as the mean
        /// length of strings is usually much less than half of the allowed maximum.
        /// The string will not start or end with white space.
        /// </summary>
        /// <param name="prefix">Optionally specifies the first few characters of the string.
        /// This allows the string to be identified amidst other randomly generated strings.</param>
        /// <param name="minLength">The minimum length of the string.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <param name="includeSpaces">When set to true, spaces will be randomly included.</param>
        /// <returns></returns>
        public static string CreateRandomString(string prefix, int minLength, int maxLength, bool includeSpaces = true)
        {
            // Get a length between min and max, weighted towards the shorter end.
            double weight = Math.Sqrt(R.NextDouble());
            int length = minLength + (int)(weight * (maxLength - minLength));

            string result = string.Empty;

            if (length > prefix.Length)
            {
                result += prefix;

                for (int index = prefix.Length; index < length; ++index)
                {
                    if (includeSpaces && index != 0 && index != (length - 1) && R.Next(6) == 0)
                    {
                        result += ' ';
                    }
                    else
                    {
                        result += (char)R.Next(65, 90);
                    }
                }
            }
            else
            {
                result += prefix.Substring(0, length);
            }

            return result;
        }

        public static readonly Random R = new Random();

        public static string CreateRandomStringOfNumbers(int minLength, int maxLength)
        {
            int length = R.Next(minLength, maxLength);

            string result = string.Empty;

            for (int index = 0; index < length; ++index)
            {
                result += (char)R.Next(48, 57);
            }

            return result;
        }
    }
}
