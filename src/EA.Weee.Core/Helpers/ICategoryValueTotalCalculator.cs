namespace EA.Weee.Core.Helpers
{
    using System.Collections.Generic;

    public interface ICategoryValueTotalCalculator
    {
        string Total(IList<string> categoryValues);
    }
}
