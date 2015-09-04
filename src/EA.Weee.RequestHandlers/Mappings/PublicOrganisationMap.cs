namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Core.Shared;
    using Domain.Organisation;
    using Prsd.Core.Mapper;

    public class PublicOrganisationMap : IMap<Organisation, PublicOrganisationData>
    {
        private readonly IMap<Address, AddressData> addressMap;

        public PublicOrganisationMap(IMap<Address, AddressData> addressMap)
        {
            this.addressMap = addressMap;
        }

        public PublicOrganisationData Map(Organisation source)
        {
            return new PublicOrganisationData
            {
                Id = source.Id,
                Address = source.OrganisationAddress != null
                    ? addressMap.Map(source.OrganisationAddress)
                    : null,
                DisplayName = source.OrganisationType == Domain.Organisation.OrganisationType.RegisteredCompany ? source.Name : source.TradingName
            };
        }
    }
}
