﻿namespace EA.Weee.RequestHandlers.Mappings
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
                DisplayName = source.OrganisationType == Domain.Organisation.OrganisationType.RegisteredCompany || source.OrganisationType == Domain.Organisation.OrganisationType.SoleTraderOrIndividual
                              ? source.Name
                              : source.TradingName,
                Address = source.BusinessAddress != null
                    ? addressMap.Map(source.BusinessAddress)
                    : null,
                NpwdMigratedComplete = source.NpwdMigratedComplete,
                NpwdMigrated = source.NpwdMigrated
            };
        }
    }
}
