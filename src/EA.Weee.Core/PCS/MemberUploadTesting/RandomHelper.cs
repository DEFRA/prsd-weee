using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
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

        public static string CreateRandomString(string prefix, int minLength, int maxLength, bool includeSpaces = true)
        {
            string result = prefix;

            // Get a length between min and max, weighted towards the shorter end.
            double weight = Math.Sqrt(R.NextDouble());
            int length = minLength + (int)(weight * (maxLength - minLength));

            while (result.Length < length)
            {
                if (includeSpaces && R.Next(6) == 0)
                {
                    result += ' ';
                }
                else
                {
                    result += (char)R.Next(65, 90);
                }
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
