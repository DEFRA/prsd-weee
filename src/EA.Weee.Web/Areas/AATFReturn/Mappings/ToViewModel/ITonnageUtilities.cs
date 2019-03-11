namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
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
