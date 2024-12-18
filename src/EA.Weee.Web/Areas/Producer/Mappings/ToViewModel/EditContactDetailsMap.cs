namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditContactDetailsMap : IMap<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>
    {
        private readonly IMapper mapper;

        public EditContactDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditContactDetailsViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            var viewModel = new EditContactDetailsViewModel
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                OrganisationId = submissionData.OrganisationData.Id,
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                RedirectToCheckAnswers = source.RedirectToCheckAnswers,
                ContactDetails = new Core.Organisations.ContactDetailsViewModel()
                {
                    AddressData = new AddressPostcodeRequiredData()
                }
            };

            if (submissionData.CurrentSubmission.ContactData != null)
            {
                viewModel.ContactDetails.FirstName = submissionData.CurrentSubmission.ContactData.FirstName;
                viewModel.ContactDetails.LastName = submissionData.CurrentSubmission.ContactData.LastName;
                viewModel.ContactDetails.Position = submissionData.CurrentSubmission.ContactData.Position;

                viewModel.ContactDetails.AddressData =
                    mapper.Map<AddressData, AddressPostcodeRequiredData>(submissionData.CurrentSubmission.ContactAddressData);
            }

            return viewModel;
        }
    }
}