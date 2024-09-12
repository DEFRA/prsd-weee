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

    public class EditContactDetailsMap : IMap<SmallProducerSubmissionData, EditContactDetailsViewModel>
    {
        private readonly IMapper mapper;

        public EditContactDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditContactDetailsViewModel Map(SmallProducerSubmissionData source)
        {
            var externalOrganisationType = MapOrganisationType(source.OrganisationData.OrganisationType);
            var businessAddressData = MapBusinessAddress(source);

            var additionalContactDetails = source.CurrentSubmission.AdditionalCompanyDetailsData.Select(c => new AdditionalContactModel()
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
            }).ToList();

            var organisation = new OrganisationViewModel
            {
                OrganisationType = externalOrganisationType,
                Address = businessAddressData,
                EEEBrandNames = source.CurrentSubmission.EEEBrandNames,
                CompanyName = source.CurrentSubmission.CompanyName,
                BusinessTradingName = source.CurrentSubmission.TradingName,
            };

            var viewModel = new EditContactDetailsViewModel
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                //Organisation = organisation,
                AdditionalContactModels = additionalContactDetails,
                HasAuthorisedRepresentitive = source.HasAuthorisedRepresentitive
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