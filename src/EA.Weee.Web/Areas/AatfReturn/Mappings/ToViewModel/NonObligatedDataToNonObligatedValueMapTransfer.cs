namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;

    public class NonObligatedDataToNonObligatedValueMapTransfer
    {
        public IList<NonObligatedData> NonObligatedDataValues { get; set; }

        public IList<NonObligatedCategoryValue> NonObligatedCategoryValues { get; set; }
    }
}