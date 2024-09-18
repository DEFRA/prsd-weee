namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;

    public class EditOrganisationDetailsMap : IMap<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>
    {
        private readonly IMapper mapper;

        public EditOrganisationDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditOrganisationDetailsViewModel Map(SmallProducerSubmissionMapperData source)
        {
            Condition.Requires(source.SmallProducerSubmissionData).IsNotNull();

            var submissionData = source.SmallProducerSubmissionData;

            var externalOrganisationType = MapOrganisationType(submissionData.OrganisationData.OrganisationType);
            var businessAddressData = MapBusinessAddress(submissionData);

            var additionalContactDetails = submissionData.CurrentSubmission.AdditionalCompanyDetailsData.Select(c => new AdditionalContactModel()
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
            }).ToList();

            var organisation = new OrganisationViewModel
            {
                OrganisationType = externalOrganisationType,
                Address = businessAddressData,
                EEEBrandNames = submissionData.CurrentSubmission.EEEBrandNames,
                CompanyName = submissionData.CurrentSubmission.CompanyName,
                BusinessTradingName = submissionData.CurrentSubmission.TradingName,
                CompaniesRegistrationNumber = submissionData.OrganisationData.CompanyRegistrationNumber
            };

            var viewModel = new EditOrganisationDetailsViewModel
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                OrganisationId = submissionData.OrganisationData.Id,
                Organisation = organisation,
                AdditionalContactModels = additionalContactDetails,
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive
            };

            return viewModel;
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

        private ExternalAddressData MapBusinessAddress(SmallProducerSubmissionData source)
        {
            return mapper.Map<AddressData, ExternalAddressData>(source.CurrentSubmission.BusinessAddressData);
        }
    }
}