﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Domain;
    using Requests.Organisations;
    using Requests.Shared;
    using AddressType = Requests.Shared.AddressType;
    using OrganisationType = Requests.Organisations.OrganisationType;

    internal class ValueObjectInitializer
    {
        public static Contact CreateContact(ContactData contact)
        {
            return new Contact(contact.FirstName, contact.LastName, contact.Position);
        }

        public static Country CreateCountry(CountryData country)
        {
            return new Country(country.Id, country.Name);
        }

        public static Address CreateAddress(AddressData address)
        {
            return new Address(address.Address1,
                address.Address2,
                address.TownOrCity,
                address.CountyOrRegion,
                address.Postcode,
                CreateCountry(address.Country),
                address.Telephone,
                address.Email);
        }

        private static Domain.OrganisationType GetOrganisationType(OrganisationType organisationType)
        {
            switch (organisationType)
            {
                case OrganisationType.RegisteredCompany:
                    return Domain.OrganisationType.RegisteredCompany;

                case OrganisationType.SoleTraderOrIndividual:
                    return Domain.OrganisationType.SoleTraderOrIndividual;

                case OrganisationType.Partnership:
                    return Domain.OrganisationType.Partnership;

                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", organisationType),
                        "organisationType");
            }
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