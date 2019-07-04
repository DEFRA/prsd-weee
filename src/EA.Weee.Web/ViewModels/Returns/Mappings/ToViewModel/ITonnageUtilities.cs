namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface ITonnageUtilities
    {
        ObligatedCategoryValue SumObligatedValues(List<WeeeObligatedData> dataSet);

        string SumTotals(List<decimal?> values);

        decimal? InitialiseTotalDecimal(decimal? tonnage);

        string CheckIfTonnageIsNull(decimal? tonnage);
    }
}
