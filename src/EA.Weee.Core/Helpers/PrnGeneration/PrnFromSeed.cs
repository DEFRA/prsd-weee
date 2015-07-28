namespace EA.Weee.Core.Helpers.PrnGeneration
{
    using System;

    public class PrnSeedHelper
    {
        public ushort Number;

        public char FirstLetter;

        public char SecondLetter;

        public char ThirdLetter;

        public char FourthLetter;

        public PrnSeedHelper(uint seed)
        {
            Number = NumberComponentOfSeed(seed);
            FirstLetter = (char)(LetterComponentOfSeed(seed, 0xF0000000, 28) + 'A');
            SecondLetter = (char)(LetterComponentOfSeed(seed, 0x0F000000, 24) + 'A');
            ThirdLetter = (char)(LetterComponentOfSeed(seed, 0x00F00000, 20) + 'M');
            FourthLetter = (char)(LetterComponentOfSeed(seed, 0x000F0000, 16) + 'M');

            PutComponentsInCorrectRanges();
        }

        public uint ToSeedValue()
        {
            return (uint)(Number | (FirstLetter - 'A') << 28 | (SecondLetter - 'A') << 24 | (ThirdLetter - 'M') << 20 | (FourthLetter - 'M') << 16);
        }

        private static ushort NumberComponentOfSeed(uint seed)
        {
            const uint NumberMask = 0x0000FFFF;
            return (ushort)(seed & NumberMask);
        }

        private static char LetterComponentOfSeed(uint seed, uint mask, int offset)
        {
            return (char)((seed & mask) >> offset);
        }

        /// <summary>
        /// As values rise, they hit the limits of their ranges and must tick over the 'next' component of the PRN.
        /// i.e. once the number hits its maximum value, it ticks back down to zero and ticks up the fourth letter.
        /// </summary>
        private void PutComponentsInCorrectRanges()
        {
            if (Number >= QuadraticResidueHelper.NumberRange)
            {
                Number = 0;
                FourthLetter++;
            }

            if (OverflowOccursWhenPuttingCharacterInRange(ref FourthLetter, 'M', QuadraticResidueHelper.CharRange))
            {
                ThirdLetter++;
            }

            if (OverflowOccursWhenPuttingCharacterInRange(ref ThirdLetter, 'M', QuadraticResidueHelper.CharRange))
            {
                SecondLetter++;
            }

            if (OverflowOccursWhenPuttingCharacterInRange(ref SecondLetter, 'A', QuadraticResidueHelper.CharRange))
            {
                FirstLetter++;
            }

            if (OverflowOccursWhenPuttingCharacterInRange(ref FirstLetter, 'A', QuadraticResidueHelper.CharRange))
            {
                throw new ArgumentOutOfRangeException("We've hit the limit on the number of PRNs we can generate with this scheme.");
            }
        }

        private bool OverflowOccursWhenPuttingCharacterInRange(ref char character, char lowerLimit, int allowableRangeSize)
        {
            if (character >= lowerLimit + allowableRangeSize)
            {
                character = lowerLimit;
                return true;
            }

            return false;
        }
    }
}
