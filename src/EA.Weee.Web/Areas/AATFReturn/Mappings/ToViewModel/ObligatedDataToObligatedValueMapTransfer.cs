namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public class ObligatedDataToObligatedValueMapTransfer
    {
        public IList<WeeeObligatedData> WeeeDataValues { get; set; }

        public IList<ObligatedCategoryValue> ObligatedCategoryValues { get; set; }
    }
}