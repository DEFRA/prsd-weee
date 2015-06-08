namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Domain;
    using Requests.Organisations;
    using Requests.Shared;
    using OrganisationType = Requests.Organisations.Type;

    internal class ValueObjectInitializer
    {
        private static Domain.OrganisationType GetOrganisationType(OrganisationType type)
        {
            switch (type)
            {
                case OrganisationType.RegisteredCompany:
                    return Domain.OrganisationType.RegisteredCompany;
                case OrganisationType.SoleTraderOrIndividual:
                    return Domain.OrganisationType.SoleTraderOrIndividual;
                case OrganisationType.Partnership:
                    return Domain.OrganisationType.Partnership;
                default:
                    throw new ArgumentException(string.Format("Unknown organisation type: {0}", type), "type");
            }
        }

        public static Contact CreateContact(ContactData contact)
        {
            return new Contact(contact.FirstName, contact.LastName, contact.Position);
        }

        public static Address CreateAddress(AddressData address, string countryName)
        {
            return new Address(address.Address1,
                address.Address2,
                address.TownOrCity,
                address.CountyOrRegion,
                address.PostalCode,
                address.Country,
                address.Telephone,
                address.Email);
        }
    }
}