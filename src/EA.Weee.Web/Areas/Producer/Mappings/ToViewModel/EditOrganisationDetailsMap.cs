namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditOrganisationDetailsMap : IMap<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>
    {
        private readonly IMapper mapper;

        public EditOrganisationDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditOrganisationDetailsViewModel Map(SmallProducerSubmissionData source)
        {
            var viewModel = new EditOrganisationDetailsViewModel();

            ExternalOrganisationType externalOrganisationType;

            switch (source.OrganisationData.OrganisationType)
            {
                case OrganisationType.Partnership:
                    externalOrganisationType = ExternalOrganisationType.Partnership;
                    break;
                case OrganisationType.RegisteredCompany:
                    externalOrganisationType = ExternalOrganisationType.RegisteredCompany;
                    break;
                case OrganisationType.SoleTraderOrIndividual:
                    externalOrganisationType = ExternalOrganisationType.SoleTrader;
                    break;
                default:
                    externalOrganisationType = ExternalOrganisationType.RegisteredCompany;
                    break;
            }

            ExternalAddressData businessAddressData = null;
            businessAddressData = mapper.Map<AddressData, ExternalAddressData>(source.CurrentSubmission.BusinessAddressData ?? source.OrganisationData.BusinessAddress);

            var organisation = new OrganisationViewModel()
            {
                OrganisationType = externalOrganisationType,
                Address = businessAddressData,
                EEEBrandNames = source.CurrentSubmission.EEEBrandNames
            };

            viewModel.Organisation = organisation;

            return viewModel;
        }
    }
}