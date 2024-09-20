namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class ToAddSignatoryRequestCreator : IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryRequest>
    {
        private readonly IMapper mapper;

        public ToAddSignatoryRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AddSignatoryRequest ViewModelToRequest(AppropriateSignatoryViewModel viewModel)
        {
            var contact = new ContactData()
            {
                FirstName = viewModel.Contact.FirstName,
                LastName = viewModel.Contact.LastName,
                Position = viewModel.Contact.Position ?? string.Empty,
            };
            return new AddSignatoryRequest(viewModel.DirectRegistrantId, contact);
        }
    }
}