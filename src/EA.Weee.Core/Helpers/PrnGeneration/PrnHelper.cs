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

        public string CreateUniqueRandomVersionOfPrn(PrnAsComponents prnAsComponents)
        {
            int firstLetterOrdinal = prnAsComponents.FirstLetter.GetOrdinalValue();
            int secondLetterOrdinal = prnAsComponents.SecondLetter.GetOrdinalValue();
            int thirdLetterOrdinal = prnAsComponents.ThirdLetter.GetOrdinalValue();
            int fourthLetterOrdinal = prnAsComponents.FourthLetter.GetOrdinalValue();

            // use quadratic residue to get a nice unique pseudorandomised version
            int randomisedNumber = quadraticResidueHelper.ForFourDigitNumber(prnAsComponents.Number);
            char randomisedFirstLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(firstLetterOrdinal) + prnAsComponents.FirstLetter.Limit);
            char randomisedSecondLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(secondLetterOrdinal) + prnAsComponents.SecondLetter.Limit);
            char randomisedThirdLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(thirdLetterOrdinal) + prnAsComponents.ThirdLetter.Limit);
            char randomisedFourthLetter = (char)(quadraticResidueHelper.ForSmallSubsetOfLetters(fourthLetterOrdinal) + prnAsComponents.FourthLetter.Limit);

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
            return new StringBuilder("WEE/")
                .Append(randomisedFirstLetter)
                .Append(randomisedSecondLetter)
                .Append(randomisedNumber.ToString("D4"))
                .Append(randomisedThirdLetter)
                .Append(randomisedFourthLetter)
                .ToString();
        }
    }
}
