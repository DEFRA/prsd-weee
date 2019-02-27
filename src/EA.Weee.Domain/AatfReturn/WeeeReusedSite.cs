namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;

    public class WeeeReusedSite
    {
        public virtual Guid WeeeReusedId { get; private set; }

        public virtual Address Address { get; private set; }

        protected WeeeReusedSite()
        {
        }

        public WeeeReusedSite(Guid weeeReusedId, Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            WeeeReusedId = weeeReusedId;
            Address = address;
        }
    }
}
