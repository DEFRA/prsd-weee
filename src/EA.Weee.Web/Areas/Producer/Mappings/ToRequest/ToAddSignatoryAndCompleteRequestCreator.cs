namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class ToAddSignatoryAndCompleteRequestCreator : IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryAndCompleteRequest>
    {
        private readonly IMapper mapper;

        public ToAddSignatoryAndCompleteRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AddSignatoryAndCompleteRequest ViewModelToRequest(AppropriateSignatoryViewModel viewModel)
        {
            var contact = new ContactData()
            {
                FirstName = viewModel.Contact.FirstName,
                LastName = viewModel.Contact.LastName,
                Position = viewModel.Contact.Position ?? string.Empty,
            };
            return new AddSignatoryAndCompleteRequest(viewModel.DirectRegistrantId, contact);
        }
    }
}