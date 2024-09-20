namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;

    public class OrganisationViewModelMap : IMap<SmallProducerSubmissionData, OrganisationViewModel>
    {
        private readonly IMapper mapper;

        public OrganisationViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public OrganisationViewModel Map(SmallProducerSubmissionData source)
        {
            return new OrganisationViewModel()
            {
                Address = mapper.Map<AddressData, ExternalAddressData>(source.OrganisationData.BusinessAddress),
                CompanyName = source.OrganisationData.Name,
                BusinessTradingName = source.OrganisationData.TradingName,
                CompaniesRegistrationNumber = source.OrganisationData.CompanyRegistrationNumber,
                OrganisationType = MapOrganisationType(source.OrganisationData.OrganisationType)
            };
        }

        private ExternalOrganisationType MapOrganisationType(OrganisationType organisationType)
        {
            switch (organisationType)
            {
                case OrganisationType.Partnership:
                    return ExternalOrganisationType.Partnership;
                case OrganisationType.RegisteredCompany:
                    return ExternalOrganisationType.RegisteredCompany;
                case OrganisationType.SoleTraderOrIndividual:
                    return ExternalOrganisationType.SoleTrader;
                default:
                    return ExternalOrganisationType.RegisteredCompany;
            }
        }
    }
}