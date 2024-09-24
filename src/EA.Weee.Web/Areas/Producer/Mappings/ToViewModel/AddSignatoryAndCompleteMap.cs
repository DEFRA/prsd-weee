namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.ViewModels.Shared;

    public class AddSignatoryAndCompleteMap : IMap<SmallProducerSubmissionData, AppropriateSignatoryViewModel>
    {
        private readonly IMapper mapper;

        public AddSignatoryAndCompleteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AppropriateSignatoryViewModel Map(SmallProducerSubmissionData source)
        {
            var viewModel = new AppropriateSignatoryViewModel
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                HasAuthorisedRepresentitive = source.HasAuthorisedRepresentitive
            };

            var contactPersonViewModel = viewModel.Contact = new ContactPersonViewModel();

            contactPersonViewModel.FirstName = source.CurrentSubmission.ContactData.FirstName;
            contactPersonViewModel.LastName = source.CurrentSubmission.ContactData.LastName;
            contactPersonViewModel.Position = source.CurrentSubmission.ContactData.Position;

            return viewModel;
        }
    }
}