namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.Organisations;
    using Core.Shared;
    using Domain;
    using Domain.Organisation;
    using AddressType = Domain.AddressType;
    using ObligationType = Domain.ObligationType;

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

        public static AddressType GetAddressType(Core.Shared.AddressType addressType)
        {
            switch (addressType)
            {
                case Core.Shared.AddressType.OrganisationAddress:
                    return AddressType.OrganisationAddress;

                case Core.Shared.AddressType.RegisteredOrPPBAddress:
                    return AddressType.RegisteredOrPPBAddress;

                case Core.Shared.AddressType.ServiceOfNotice:
                    return AddressType.ServiceOfNoticeAddress;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", addressType),
                        "addressType");
            }
        }

        public static ObligationType GetObligationType(Core.Shared.ObligationType obligationType)
        {
            switch (obligationType)
            {
                case Core.Shared.ObligationType.B2B:
                    return ObligationType.B2B;

                case Core.Shared.ObligationType.B2C:
                    return ObligationType.B2C;

                case Core.Shared.ObligationType.Both:
                    return ObligationType.Both;

                default:
                    throw new ArgumentException(string.Format("Unknown obligation type: {0}", obligationType),
                        "obligationType");
            }
        }
    }
}