namespace EA.Weee.Core.Helpers.PrnGeneration
{
    using System.Text;

    public class PrnHelper : IPrnHelper
    {
        private readonly IQuadraticResidueHelper quadraticResidueHelper;

        public PrnHelper(IQuadraticResidueHelper quadraticResidueHelper)
        {
            this.quadraticResidueHelper = quadraticResidueHelper;
        }

        /// <summary>
        /// Takes a non-negative integer, generates a unique pseudorandom PRN based on
        /// that integer, and returns the next integer that can be used to generate
        /// the next PRN. Technical details are in QuadraticResidueHelper.cs.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public string ComputePrnFromSeed(ref uint seed)
        {
            // generate a plain unrandomised PRN from the seed, e.g. '1' => 'AA0001AA', '3' => 'AA0003AA'
            PrnSeedHelper prnSeedHelper = new PrnSeedHelper(seed + 1);

            int firstLetterOrdinal = prnSeedHelper.FirstLetter - 'A';
            int secondLetterOrdinal = prnSeedHelper.SecondLetter - 'A';
            int thirdLetterOrdinal = prnSeedHelper.ThirdLetter - 'M';
            int fourthLetterOrdinal = prnSeedHelper.FourthLetter - 'M';

            // use quadratic residue to get a nice unique pseudorandomised version
            int randomisedNumber = quadraticResidueHelper.ForFourDigitNumber(prnSeedHelper.Number);
            char randomisedFirstLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(firstLetterOrdinal) + 'A');
            char randomisedSecondLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(secondLetterOrdinal) + 'A');
            char randomisedThirdLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(thirdLetterOrdinal) + 'M');
            char randomisedFourthLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(fourthLetterOrdinal) + 'M');

            // there's a few characters we want to skip to avoid confusion with numbers
            if (randomisedFirstLetter >= 'I')
            {
                randomisedFirstLetter++;
            }

            if (randomisedSecondLetter >= 'I')
            {
                randomisedSecondLetter++;
            }

            if (randomisedThirdLetter >= 'O')
            {
                randomisedThirdLetter++;
            }

            if (randomisedFourthLetter >= 'O')
            {
                randomisedFourthLetter++;
            }

            // all done, toss the pseudorandomised PRN back out along with the next seed value
            var prn = new StringBuilder("WEE/")
                .Append(randomisedFirstLetter)
                .Append(randomisedSecondLetter)
                .Append(randomisedNumber.ToString("D4"))
                .Append(randomisedThirdLetter)
                .Append(randomisedFourthLetter)
                .ToString();

            seed = prnSeedHelper.ToSeedValue();

            return prn;
        }
    }
}
