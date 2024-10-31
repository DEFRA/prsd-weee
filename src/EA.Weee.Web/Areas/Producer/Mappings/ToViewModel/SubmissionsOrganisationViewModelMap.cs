namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;

    public class SubmissionsOrganisationViewModelMap : IMap<SubmissionsYearDetails, OrganisationViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsOrganisationViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public OrganisationViewModel Map(SubmissionsYearDetails source)
        {
            if (source.Year.HasValue)
            {
                var sub = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

                return new OrganisationViewModel()
                {
                    Status = sub.Status,
                    HasPaid = sub.HasPaid,
                    RegistrationDate = sub.RegistrationDate,
                    SubmittedDate = sub.SubmittedDate,
                    DirectProducerSubmissionId = sub.DirectProducerSubmissionId,
                    PaymentReference = sub.PaymentReference,
                    ProducerRegistrationNumber = sub.ProducerRegistrationNumber,
                    Address = mapper.Map<AddressData, ExternalAddressData>(sub.BusinessAddressData),
                    CompanyName = sub.CompanyName,
                    BusinessTradingName = sub.TradingName,
                    CompaniesRegistrationNumber = source.SmallProducerSubmissionData.OrganisationData.CompanyRegistrationNumber,
                    OrganisationType = MapOrganisationType(source.SmallProducerSubmissionData.OrganisationData.OrganisationType)
                };
            }

            return new OrganisationViewModel()
            {
                ProducerRegistrationNumber = source.SmallProducerSubmissionData.ProducerRegistrationNumber,
                Address = mapper.Map<AddressData, ExternalAddressData>(source.SmallProducerSubmissionData.OrganisationData.BusinessAddress),
                CompanyName = source.SmallProducerSubmissionData.OrganisationData.Name,
                BusinessTradingName = source.SmallProducerSubmissionData.OrganisationData.TradingName,
                CompaniesRegistrationNumber = source.SmallProducerSubmissionData.OrganisationData.CompanyRegistrationNumber,
                OrganisationType = MapOrganisationType(source.SmallProducerSubmissionData.OrganisationData.OrganisationType)
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