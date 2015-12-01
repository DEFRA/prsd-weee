namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.Organisations;
    using Core.Shared;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using OrganisationStatus = Core.Shared.OrganisationStatus;
    using OrganisationType = Core.Organisations.OrganisationType;

    public class OrganisationMap : IMap<Organisation, OrganisationData>
    {
        private readonly IMap<Address, AddressData> addressMap;
        private readonly IMap<Contact, ContactData> contactMap;

        public OrganisationMap(IMap<Address, AddressData> addressMap, IMap<Contact, ContactData> contactMap)
        {
            this.addressMap = addressMap;
            this.contactMap = contactMap;
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
                    (OrganisationType)
                        Enum.Parse(typeof(OrganisationType),
                            source.OrganisationType.Value.ToString()),

                // Use existing mappers to map addresses and contact
                BusinessAddress = source.BusinessAddress != null
                    ? addressMap.Map(source.BusinessAddress)
                    : null,
                Contact = source.Contact != null
                    ? contactMap.Map(source.Contact)
                    : null,
                NotificationAddress = source.NotificationAddress != null
                    ? addressMap.Map(source.NotificationAddress)
                    : null,
                OrganisationAddress = source.OrganisationAddress != null
                    ? addressMap.Map(source.OrganisationAddress)
                    : null,
                HasOrganisationAddress = source.HasOrganisationAddress,
                HasBusinessAddress = source.HasBusinessAddress,
                HasNotificationAddress = source.HasNotificationAddress,
                OrganisationName = source.OrganisationName,
            };
        }
    }
}
