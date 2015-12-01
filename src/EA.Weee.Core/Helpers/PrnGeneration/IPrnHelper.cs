namespace EA.Weee.Core.Helpers.PrnGeneration
{
    public interface IPrnHelper
    {
        string CreateUniqueRandomVersionOfPrn(PrnAsComponents prnAsComponents);
    }
}