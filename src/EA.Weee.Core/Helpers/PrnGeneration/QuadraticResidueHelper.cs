namespace EA.Weee.Core.Helpers.PrnGeneration
{
    using System;

    public class QuadraticResidueHelper : IQuadraticResidueHelper
    {
        public const int NumberRange = 9967;

        public const int CharRange = 11;

        public int ForFourDigitNumber(int value)
        {
            return QuadraticResidue(value, NumberRange);
        }

        public int ForSmallSubsetOfLetters(int value)
        {
            return QuadraticResidue(value, CharRange);
        }
        
        /// <summary>
        /// Takes an int x, where x is less than an int prime, and returns another value that is also less than prime.
        /// This second value is guaranteed to be unique to each x as long as x is between zero inclusive and prime
        /// exclusive, and as long as (prime - 3) % 4 == 0.
        /// http://preshing.com/20121224/how-to-generate-a-sequence-of-unique-random-integers/
        /// </summary>
        /// <param name="x"></param>
        /// <param name="prime"></param>
        /// <returns></returns>
        public int QuadraticResidue(int x, int prime)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException("x must be higher than zero");
            }

            if (x >= prime)
            {
                throw new ArgumentOutOfRangeException("x must be less than prime");
            }

            if ((prime - 3) % 4 != 0)
            {
                throw new ArgumentOutOfRangeException("prime must obey the constraint: (prime - 3) % 4 == 0");
            }

            int residue = (x * x) % prime;

            return (x <= prime / 2) ? residue : prime - residue;
        }
    }
}
