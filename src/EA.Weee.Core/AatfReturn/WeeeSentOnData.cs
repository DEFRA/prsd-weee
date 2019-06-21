namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;

    public class WeeeSentOnData
    {
        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }

        public virtual Guid WeeeSentOnId { get; set; }

        public virtual Guid ReturnId { get; set; }

        public virtual Guid AatfId { get; set; }

        public virtual Guid SiteAddressId { get; set; }

        public virtual bool Removed { get; set; }

        public virtual Guid OperatorAddressId { get; set; }

        public List<WeeeObligatedData> Tonnages { get; set; }

        public WeeeSentOnData()
        {
        }
    }
}
