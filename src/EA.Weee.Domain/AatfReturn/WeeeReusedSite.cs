namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;

    public class WeeeReusedSite
    {
        public virtual WeeeReused WeeeReused { get; private set; }

        public virtual Address Address { get; private set; }

        protected WeeeReusedSite()
        {
        }

        public WeeeReusedSite(WeeeReused weeeReused, Address address)
        {
            Guard.ArgumentNotNull(() => weeeReused, weeeReused);
            Guard.ArgumentNotNull(() => address, address);

            WeeeReused = weeeReused;
            Address = address;
        }
    }
}
