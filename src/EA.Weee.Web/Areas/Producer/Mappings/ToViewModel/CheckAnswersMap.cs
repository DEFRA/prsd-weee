namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class CheckAnswersMap : IMap<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
    {
        private readonly IMapper mapper;

        public CheckAnswersMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CheckAnswersViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            var editOrganisationDetailsmodel =
                            mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>(source);
            var editContactDetailsmodel =
                            mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>(source);
            var serviceOfNoticemodel =
                            mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(source);

            RepresentingCompanyDetailsViewModel representingCompanyDetailsmodel = null;

            if (submissionData.HasAuthorisedRepresentitive)
            {
                representingCompanyDetailsmodel =
                            mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(source);
            }
            var editEeeDatamodel =
                            mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(source);

            var viewModel = new CheckAnswersViewModel()
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                OrganisationId = submissionData.OrganisationData.Id,
                OrganisationDetails = editOrganisationDetailsmodel,
                ContactDetails = editContactDetailsmodel,
                ServiceOfNoticeData = serviceOfNoticemodel,
                RepresentingCompanyDetails = representingCompanyDetailsmodel,
                EeeData = editEeeDatamodel,
            };

            return viewModel;
        }
    }
}