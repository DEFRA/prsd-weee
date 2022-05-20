namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using System;
    using System.Collections.Generic;

    public class WeeeSentOn : AatfEntity, IReturnOption
    {
        public virtual AatfAddress OperatorAddress { get; private set; }

        public virtual Guid OperatorAddressId { get; private set; }

        public virtual AatfAddress SiteAddress { get; private set; }

        public virtual Guid SiteAddressId { get; private set; }        

        public IList<WeeeSentOnAmount> WeeeSentOnAmounts { get; set; }

        public virtual void UpdateWithOperatorAddress(AatfAddress address)
        {
            OperatorAddress = address;
        }

        public WeeeSentOn()
        {
        }

        public WeeeSentOn(Guid returnId, Guid aatfId, Guid operatorAddressId, Guid siteAddressId)
        {
            this.ReturnId = returnId;
            this.AatfId = aatfId;
            this.OperatorAddressId = operatorAddressId;
            this.SiteAddressId = siteAddressId;
        }

        public WeeeSentOn(Guid siteAddress, Guid aatf, Guid @return)
        {
            this.SiteAddressId = siteAddress;
            this.AatfId = aatf;
            this.ReturnId = @return;
        }

        public WeeeSentOn(AatfAddress operatorAddress, AatfAddress siteAddress, Aatf aatf, Return @return)
        {
            Guard.ArgumentNotNull(() => siteAddress, siteAddress);
            Guard.ArgumentNotNull(() => operatorAddress, operatorAddress);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => @return, @return);

            this.SiteAddress = siteAddress;
            this.OperatorAddress = operatorAddress;
            this.Aatf = aatf;
            this.Return = @return;
        }

        public WeeeSentOn(AatfAddress siteAddress, Aatf aatf, Return @return)
        {
            Guard.ArgumentNotNull(() => siteAddress, siteAddress);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => @return, @return);

            this.SiteAddress = siteAddress;
            this.Aatf = aatf;
            this.Return = @return;
        }
    }
}
