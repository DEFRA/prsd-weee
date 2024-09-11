namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Core.Shared;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using System;
    using OrganisationStatus = Core.Shared.OrganisationStatus;
    using OrganisationType = Core.Organisations.OrganisationType;

    public class OrganisationMap : IMap<Organisation, OrganisationData>
    {
        private readonly IMap<Address, AddressData> addressMap;

        public OrganisationMap(IMap<Address, AddressData> addressMap)
        {
            this.addressMap = addressMap;
        }

        public OrganisationData Map(Organisation source)
        {
            return new OrganisationData
            {
                Id = source.Id,
                RowVersion = source.RowVersion,
                CompanyRegistrationNumber = source.CompanyRegistrationNumber,
                Name = source.Name,
                TradingName = source.TradingName,

                // SQL doesn't allow nulls so no chance of null ref exception for enums
                OrganisationStatus =
                    (OrganisationStatus)
                        Enum.Parse(typeof(OrganisationStatus), source.OrganisationStatus.Value.ToString()),
                OrganisationType =
                    MapOrganisationType(source.OrganisationType),
                // Use existing mappers to map addresses
                BusinessAddress = source.BusinessAddress != null
                    ? addressMap.Map(source.BusinessAddress)
                    : null,
                NotificationAddress = source.NotificationAddress != null
                    ? addressMap.Map(source.NotificationAddress)
                    : null,
                HasBusinessAddress = source.HasBusinessAddress,
                HasNotificationAddress = source.HasNotificationAddress,
                OrganisationName = source.OrganisationName,
                IsBalancingScheme = source.ProducerBalancingScheme != null
            };
        }

        private OrganisationType MapOrganisationType(Domain.Organisation.OrganisationType organisationType)
        {
            if (organisationType == Domain.Organisation.OrganisationType.DirectRegistrantPartnership)
            {
                return OrganisationType.Partnership;
            }

            return (OrganisationType)Enum.Parse(typeof(OrganisationType), organisationType.Value.ToString());
        }
    }
}
