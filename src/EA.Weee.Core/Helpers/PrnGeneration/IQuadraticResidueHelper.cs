namespace EA.Weee.Core.Helpers.PrnGeneration
{
    public interface IQuadraticResidueHelper
    {
        int ForFourDigitNumber(int value);

        int ForSmallSubsetOfLetters(int value);

        int QuadraticResidue(int x, int prime);
    }
}