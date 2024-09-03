namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;

    public partial class DirectRegistrant
    {
        public void AddOrUpdateAddress(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            Address = address.OverwriteWhereNull(Address);
        }
    }
}
