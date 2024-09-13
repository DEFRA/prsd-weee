namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditContactDetailsMap : IMap<SmallProducerSubmissionData, EditContactDetailsViewModel>
    {
        private readonly IMapper mapper;

        public EditContactDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditContactDetailsViewModel Map(SmallProducerSubmissionData source)
        {
            var viewModel = new EditContactDetailsViewModel
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                HasAuthorisedRepresentitive = source.HasAuthorisedRepresentitive
            };

            var contactDetailsViewModel = viewModel.ContactDetails = new ContactDetailsViewModel();

            contactDetailsViewModel.FirstName = source.CurrentSubmission.ContactData.FirstName;
            contactDetailsViewModel.LastName = source.CurrentSubmission.ContactData.LastName;
            contactDetailsViewModel.Position = source.CurrentSubmission.ContactData.Position;

            contactDetailsViewModel.AddressData =
                mapper.Map<AddressData, AddressPostcodeRequiredData>(source.CurrentSubmission.ContactAddressData);

            return viewModel;
        }
    }
}