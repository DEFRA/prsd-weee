namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;

    public class WeeeReusedSite
    {
        public Guid Id { get; private set; }

        public Guid WeeeReusedId { get; private set; }

        public virtual AatfAddress Address { get; private set; }

        protected WeeeReusedSite()
        {
        }

        public WeeeReusedSite(Guid weeeReusedId, AatfAddress address)
        {
            Guard.ArgumentNotNull(() => address, address);

            WeeeReusedId = weeeReusedId;
            Address = address;
        }
    }
}
