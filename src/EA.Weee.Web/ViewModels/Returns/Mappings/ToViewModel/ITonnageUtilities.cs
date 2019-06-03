namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public interface ITonnageUtilities
    {
        ObligatedCategoryValue SumObligatedValues(List<WeeeObligatedData> dataSet);

        decimal? InitialiseTotalDecimal(decimal? tonnage);

        string CheckIfTonnageIsNull(decimal? tonnage);
    }
}
