namespace EA.Weee.Core.Helpers
{
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public interface ISumCategoryTonnageValues
    {
        string SumTonnageValues<T>(IList<T> categoryValues) where T : CategoryValue;
    }
}
