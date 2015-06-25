namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Domain;
    using Requests.Organisations;
    using Requests.Shared;
    using AddressType = Requests.Shared.AddressType;
  
    internal class ValueObjectInitializer
    {
        public static Contact CreateContact(ContactData contact)
        {
            return new Contact(contact.FirstName, contact.LastName, contact.Position);
        }
    
        public static Address CreateAddress(AddressData address, Country country)
        {
            return new Address(address.Address1,
                address.Address2,
                address.TownOrCity,
                address.CountyOrRegion,
                address.Postcode,
                country,
                address.Telephone,
                address.Email);
        }

        public static Domain.AddressType GetAddressType(AddressType addressType)
        {
            switch (addressType)
            {
                case AddressType.OrganistionAddress:
                    return Domain.AddressType.OrganisationAddress;

                case AddressType.RegisteredorPPBAddress:
                    return Domain.AddressType.RegisteredOrPPBAddress;

                case AddressType.ServiceOfNotice:
                    return Domain.AddressType.ServiceOfNoticeAddress;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", addressType),
                        "addressType");
            }
        }
    }
}