namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using System;
    using ObligationType = Core.Shared.ObligationType;
    using OrganisationType = Domain.Organisation.OrganisationType;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class SchemeMap : IMap<Scheme, SchemeData>
    {
        private readonly IMapper mapper;
        private readonly IMap<Address, AddressData> addressMap;
        private readonly IMap<Contact, ContactData> contactMap;

        public SchemeMap(IMapper mapper, IMap<Address, AddressData> addressMap, IMap<Contact, ContactData> contactMap)
        {
            this.mapper = mapper;
            this.addressMap = addressMap;
            this.contactMap = contactMap;
        }

        public SchemeData Map(Scheme source)
        {
            return new SchemeData
            {
                Id = source.Id,
                OrganisationId = source.OrganisationId,
                Name = source.Organisation.OrganisationType.Value == OrganisationType.RegisteredCompany.Value || source.Organisation.OrganisationType.Value == OrganisationType.SoleTraderOrIndividual.Value
                    ? source.Organisation.Name
                    : source.Organisation.TradingName,
                SchemeStatus =
                    (SchemeStatus)
                        Enum.Parse(typeof(SchemeStatus), source.SchemeStatus.Value.ToString()),
                SchemeName = source.SchemeName,
                ApprovalName = source.ApprovalNumber,
                IbisCustomerReference = source.IbisCustomerReference,
                ObligationType = source.ObligationType != null ? (ObligationType)Enum.Parse(typeof(ObligationType), source.ObligationType.Value.ToString()) : (ObligationType?)null,
                CompetentAuthorityId = source.CompetentAuthorityId,
                CompetentAuthority = source.CompetentAuthority != null
                    ? mapper.Map<Domain.UKCompetentAuthority, UKCompetentAuthorityData>(source.CompetentAuthority)
                    : null,
                Contact = source.Contact != null
                    ? contactMap.Map(source.Contact)
                    : null,
                Address = source.Address != null
                    ? addressMap.Map(source.Address)
                    : null,
                HasAddress = source.HasAddress,
                HasContact = source.HasContact,
                IsBalancingScheme = source.Organisation.ProducerBalancingScheme != null ? true : false
            };
        }
    }
}
