namespace EA.Weee.Core.AatfReturn
{
    using System;

    public class WeeeSentOnSummaryListData
    {
        public ObligatedCategoryValue Tonnages { get; set; }

        public virtual Guid WeeeSentOnId { get; set; }

        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual string OperatorAddressLong { get; set; }

        public virtual string SiteAddressLong { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        public virtual bool Removed { get; set; }
    }
}
