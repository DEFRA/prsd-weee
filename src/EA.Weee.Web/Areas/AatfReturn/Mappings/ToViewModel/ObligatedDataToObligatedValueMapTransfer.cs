namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using System.Collections.Generic;

    public class ObligatedDataToObligatedValueMapTransfer
    {
        public IList<WeeeObligatedData> WeeeDataValues { get; set; }

        public IList<ObligatedCategoryValue> ObligatedCategoryValues { get; set; }
    }
}