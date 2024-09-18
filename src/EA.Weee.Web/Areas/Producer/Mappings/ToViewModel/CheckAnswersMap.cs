namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class CheckAnswersMap : IMap<SmallProducerSubmissionData, CheckAnswersViewModel>
    {
        private readonly IMapper mapper;

        public CheckAnswersMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CheckAnswersViewModel Map(SmallProducerSubmissionData source)
        {
            var editOrganisationDetailsmodel =
                            mapper.Map<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>(source);
            var editContactDetailsmodel =
                            mapper.Map<SmallProducerSubmissionData, EditContactDetailsViewModel>(source);
            var serviceOfNoticemodel =
                            mapper.Map<SmallProducerSubmissionData, ServiceOfNoticeViewModel>(source);

            RepresentingCompanyDetailsViewModel representingCompanyDetailsmodel = null;

            if (source.HasAuthorisedRepresentitive)
            {
                representingCompanyDetailsmodel =
                            mapper.Map<SmallProducerSubmissionData, RepresentingCompanyDetailsViewModel>(source);
            }
            var editEeeDatamodel =
                            mapper.Map<SmallProducerSubmissionData, EditEeeDataViewModel>(source);

            var viewModel = new CheckAnswersViewModel()
            {
                DirectRegistrantId = source.DirectRegistrantId,
                HasAuthorisedRepresentitive = source.HasAuthorisedRepresentitive,
                OrganisationId = source.OrganisationData.Id,
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