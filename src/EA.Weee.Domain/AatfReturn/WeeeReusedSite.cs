namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeReusedSite : Entity
    {
        public virtual WeeeReused WeeeReused { get; private set; }

        public virtual AatfAddress Address { get; private set; }

        public WeeeReusedSite()
        {
        }

        public WeeeReusedSite(WeeeReused weeeReused, AatfAddress address)
        {
            Guard.ArgumentNotNull(() => weeeReused, weeeReused);
            Guard.ArgumentNotNull(() => address, address);

            WeeeReused = weeeReused;
            Address = address;
        }
    }
}
