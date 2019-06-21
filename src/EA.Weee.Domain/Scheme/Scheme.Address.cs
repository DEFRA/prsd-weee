namespace EA.Weee.Domain.Scheme
{
    using Organisation;
    using Prsd.Core;

    public partial class Scheme
    {
        public void AddOrUpdateAddress(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            Address = address.OverwriteWhereNull(Address);
        }
    }
}
