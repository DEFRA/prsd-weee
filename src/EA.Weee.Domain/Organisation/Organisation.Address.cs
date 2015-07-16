namespace EA.Weee.Domain.Organisation
{
    using System;
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
                    if (address.IsUkAddress())
                    {
                        BusinessAddress = address.OverwriteWhereNull(BusinessAddress);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot add Business address to Organisation. Address should be UK address.");
                    }
                    break;
                case "Service of notice address":
                    NotificationAddress = address.OverwriteWhereNull(NotificationAddress);
                    break;
            }
        }
    }
}
