namespace EA.Weee.Core.Helpers.PrnGeneration
{
    using System;
    using System.Text;

    public class PrnAsComponents
    {
        public ushort Number;

        public class LetterComponent
        {
            public int Offset { get; private set; }
            public uint Mask { get; private set; }
            public char BaseChar { get; private set; }

            private char letter;

            public LetterComponent(int offset, uint mask, char baseChar)
            {
                Offset = offset;
                Mask = mask;
                BaseChar = baseChar;
            }

            public void SetLetterFromSeed(uint seed)
            {
                letter = (char)(((seed & Mask) >> Offset) + BaseChar);
            }

            public void IncrementLetter()
            {
                letter++;
            }

            public int GetLetterIntValue()
            {
                return (letter - BaseChar) << Offset;
            }

            public int GetOrdinalValue()
            {
                return letter - BaseChar;
            }

            public bool OverflowsOutOfAllowableRange()
            {
                if (letter >= BaseChar + QuadraticResidueHelper.CharRange)
                {
                    letter = BaseChar;
                    return true;
                }

                return false;
            }
        }

        public LetterComponent FirstLetter = new LetterComponent(28, 0xF0000000, 'M');
        public LetterComponent SecondLetter = new LetterComponent(24, 0x0F000000, 'M');
        public LetterComponent ThirdLetter = new LetterComponent(20, 0x00F00000, 'A');
        public LetterComponent FourthLetter = new LetterComponent(16, 0x000F0000, 'A');

        public PrnAsComponents(uint seed)
        {
            Number = NumberComponentOfSeed(seed);

            FirstLetter.SetLetterFromSeed(seed);
            SecondLetter.SetLetterFromSeed(seed);
            ThirdLetter.SetLetterFromSeed(seed);
            FourthLetter.SetLetterFromSeed(seed);

            PutComponentsInCorrectRanges();
        }

        public override string ToString()
        {
            return new StringBuilder("WEE/")
                        .Append(FirstLetter)
                        .Append(SecondLetter)
                        .Append(Number.ToString("D4"))
                        .Append(ThirdLetter)
                        .Append(FourthLetter)
                        .ToString();
        }

        public uint ToSeedValue()
        {
            return
                (uint)
                (Number
                | FirstLetter.GetLetterIntValue()
                | SecondLetter.GetLetterIntValue()
                | ThirdLetter.GetLetterIntValue()
                | FourthLetter.GetLetterIntValue());
        }

        private static ushort NumberComponentOfSeed(uint seed)
        {
            const uint NumberMask = 0x0000FFFF;
            return (ushort)(seed & NumberMask);
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
                FourthLetter.IncrementLetter();
            }

            if (FourthLetter.OverflowsOutOfAllowableRange())
            {
                ThirdLetter.IncrementLetter();
            }

            if (ThirdLetter.OverflowsOutOfAllowableRange())
            {
                SecondLetter.IncrementLetter();
            }

            if (SecondLetter.OverflowsOutOfAllowableRange())
            {
                FirstLetter.IncrementLetter();
            }

            if (FirstLetter.OverflowsOutOfAllowableRange())
            {
                throw new ArgumentOutOfRangeException("We've hit the limit on the number of PRNs we can generate with this scheme.");
            }
        }
    }
}
