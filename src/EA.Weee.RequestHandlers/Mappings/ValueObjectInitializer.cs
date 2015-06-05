namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Requests.Shared;

    internal class ValueObjectInitializer
    {  
        public static Address CreateAddress(AddressData address)
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
