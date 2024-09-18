namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.Producer;
    using Prsd.Core.Mapper;

    public class AuthorisedRepresentitiveDataMap : IMap<AuthorisedRepresentative, AuthorisedRepresentitiveData>
    {
        public AuthorisedRepresentitiveData Map(AuthorisedRepresentative source)
        {
            Condition.Requires(source).IsNotNull();

            return new AuthorisedRepresentitiveData()
            {
                CompanyName = source.OverseasProducerName,
                BusinessTradingName = source.OverseasProducerTradingName,
                Address1 = source.OverseasContact.Address.PrimaryName,
                Address2 = source.OverseasContact.Address.Street,
                CountryId = source.OverseasContact.Address.CountryId,
                CountyOrRegion = source.OverseasContact.Address.AdministrativeArea,
                TownOrCity = source.OverseasContact.Address.Town,
                Email = source.OverseasContact.Email,
                Telephone = source.OverseasContact.Telephone,
                Postcode = source.OverseasContact.Address.PostCode
            };
        }
    }
}
