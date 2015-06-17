namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;

    public partial class Organisation
    {
        public void AddAddress(AddressType type, Address address)
        {
            Guard.ArgumentNotNull(address);
            
            if (address != null)
            {
                Address addr = new Address(address.Address1, address.Address2, address.TownOrCity,
                    address.CountyOrRegion,
                    address.PostCode, address.Country, address.Telephone, address.Email);
                switch (type.Value)
                {
                    case 1:
                        if (OrganisationAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Organisation address to Organisation {0}. This organisation already has a organisation address {1}.",
                                                    this.Id,
                                                    this.OrganisationAddress.Id));
                        }
                        OrganisationAddress = addr;
                        break;
                    case 2:
                        if (BusinessAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Business address to Organisation {0}. This organisation already has a business address {1}.",
                                                    this.Id,
                                                    this.BusinessAddress.Id));
                        }
                        BusinessAddress = addr;
                        break;
                    case 3:
                        if (NotificationAddress != null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot add Notification address to Organisation {0}. This organisation already has a notification address {1}.",
                                                    this.Id,
                                                    this.NotificationAddress.Id));
                        }
                        NotificationAddress = addr;
                        break;
                }
            }
        }
    }
}