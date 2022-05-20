namespace EA.Weee.Core.AatfReturn
{
    using System;

    public class WeeeSearchedAnAatfListData
    {
        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        public virtual string ApprovalNumber { get; set; }

        public virtual Guid WeeeSentOnId { get; set; }
    }
}
