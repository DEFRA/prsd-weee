namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class ToEditContactDetailsRequestCreator : IRequestCreator<EditContactDetailsViewModel, EditContactDetailsRequest>
    {
        private readonly IMapper mapper;

        public ToEditContactDetailsRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditContactDetailsRequest ViewModelToRequest(EditContactDetailsViewModel viewModel)
        {
            var contact = new ContactData()
            {
                FirstName = viewModel.ContactDetails.FirstName,
                LastName = viewModel.ContactDetails.LastName,
                Position = viewModel.ContactDetails.Position ?? string.Empty,
            };

            var address = mapper.Map<AddressPostcodeRequiredData, AddressData>(viewModel.ContactDetails.AddressData);

            return new EditContactDetailsRequest(viewModel.DirectRegistrantId, address, contact);
        }
    }
}