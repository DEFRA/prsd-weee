namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;

    public partial class Organisation
    {
        public void AddAddress(AddressType type, Address address)
        {
            Guard.ArgumentNotNull(() => address, address);
            Guard.ArgumentNotNull(() => type, type);

            if (address != null)
            {
                Address addr = new Address(address.Address1, address.Address2, address.TownOrCity,
                    address.CountyOrRegion,
                    address.Postcode, address.Country, address.Telephone, address.Email);
                switch (type.DisplayName)
                {
                    case "Organisation address":
                        if (OrganisationAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Organisation address to Organisation {0}. This organisation already has a organisation address",
                                                    this.Id));
                        }
                        OrganisationAddress = addr;
                        break;
                    case "Registered or PPB address":
                        if (BusinessAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Business address to Organisation {0}. This organisation already has a business address.",
                                                    this.Id));
                        }
                        if (addr.IsUkAddress)
                        {
                            BusinessAddress = addr;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Business address to Organisation {0}. Address should be UK address.",
                                                    this.Id));
                        }
                        break;
                    case "Service of notice address":
                        if (NotificationAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Notification address to Organisation {0}. This organisation already has a notification address.",
                                                    this.Id));
                        }
                        NotificationAddress = addr;
                        break;
                }
            }
        }
    }
}