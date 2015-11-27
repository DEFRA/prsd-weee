namespace EA.Weee.Domain.Organisation
{
    using Prsd.Core;

    public partial class Organisation
    {
        public void AddOrUpdateAddress(AddressType type, Address address)
        {
            Guard.ArgumentNotNull(() => address, address);
            Guard.ArgumentNotNull(() => type, type);

            switch (type.DisplayName)
            {
                case "Organisation address":
                    OrganisationAddress = address.OverwriteWhereNull(OrganisationAddress);
                    break;
                case "Registered or PPB address":
                    BusinessAddress = address.OverwriteWhereNull(BusinessAddress);
                    break;
                case "Service of notice address":
                    NotificationAddress = address.OverwriteWhereNull(NotificationAddress);
                    break;
            }
        }
    }
}
