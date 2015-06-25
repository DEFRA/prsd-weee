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

                switch (type.DisplayName)
                {
                    case "Organisation address":
                        if (OrganisationAddress != null)
                        {
                            throw new InvalidOperationException("Cannot add Organisation address to Organisation. This organisation already has a organisation address");
                        }
                        OrganisationAddress = address;
                        break;
                    case "Registered or PPB address":
                        if (BusinessAddress != null)
                        {
                            throw new InvalidOperationException(
                                "Cannot add Business address to Organisation. This organisation already has a business address.");
                        }
                        if (address.IsUkAddress())
                        {
                            BusinessAddress = address;
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot add Business address to Organisation. Address should be UK address.");
                        }
                break;
                    case "Service of notice address":
                        if (NotificationAddress != null)
                        {
                            throw new InvalidOperationException("Cannot add Notification address to Organisation. This organisation already has a notification address.");
                        }
                        NotificationAddress = address;
                        break;
                }
            }
        }
    }