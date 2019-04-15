namespace EA.Weee.Core.AatfReturn
{
    public class WeeeSentOnSummaryListData
    {
        public ObligatedCategoryValue Tonnages { get; set; }

        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual string OperatorAddressLong { get; set; }

        public virtual string SiteAddressLong { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }
    }
}
