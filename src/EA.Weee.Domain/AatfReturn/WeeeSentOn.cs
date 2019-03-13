namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeSentOn : Entity
    {
        public virtual AatfAddress OperatorAddress { get; private set; }

        public virtual AatfAddress SiteAddress { get; private set; }

        public virtual Aatf Aatf { get; private set; }

        public virtual Return @Return { get; private set; }

        public WeeeSentOn()
        {
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
