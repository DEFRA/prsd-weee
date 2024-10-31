namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class ToEditOrganisationDetailsRequestCreator : IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>
    {
        private readonly IMapper mapper;

        public ToEditOrganisationDetailsRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditOrganisationDetailsRequest ViewModelToRequest(EditOrganisationDetailsViewModel viewModel)
        {
            var address = mapper.Map<ExternalAddressData, AddressData>(viewModel.Organisation.Address);

            return new EditOrganisationDetailsRequest(viewModel.DirectRegistrantId, viewModel.Organisation.CompanyName, viewModel.Organisation.BusinessTradingName,
                address, viewModel.Organisation.EEEBrandNames);
        }
    }
}