namespace EA.Weee.Core.Helpers.PrnGeneration
{
    public interface IPrnHelper
    {
        string ComputePrnFromSeed(ref uint seed);
    }
}