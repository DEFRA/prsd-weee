namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System;

    public class CheckAnswersMap : IMap<SubmissionsYearDetails, CheckAnswersViewModel>
    {
        private readonly IMapper mapper;

        public CheckAnswersMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CheckAnswersViewModel Map(SubmissionsYearDetails source, int year)
        {
            var submissionData = source.SmallProducerSubmissionData;

            var organisationDetailsmodel =
                            mapper.Map<SubmissionsYearDetails, OrganisationViewModel>(source);
            var contactDetailsmodel =
                            mapper.Map<SubmissionsYearDetails, ContactDetailsViewModel>(source);
            var serviceOfNoticemodel =
                            mapper.Map<SubmissionsYearDetails, ServiceOfNoticeViewModel>(source);

            RepresentingCompanyDetailsViewModel representingCompanyDetailsmodel = null;

            if (submissionData.HasAuthorisedRepresentitive)
            {
                representingCompanyDetailsmodel =
                            mapper.Map<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>(source);
            }
            var editEeeDatamodel =
                            mapper.Map<SubmissionsYearDetails, EditEeeDataViewModel>(source);

            var viewModel = new CheckAnswersViewModel()
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                OrganisationId = submissionData.OrganisationData.Id,
                OrganisationDetails = organisationDetailsmodel,
                ContactDetails = contactDetailsmodel,
                ServiceOfNoticeData = serviceOfNoticemodel,
                RepresentingCompanyDetails = representingCompanyDetailsmodel,
                EeeData = editEeeDatamodel,
                ComplianceYear = year,
                DisplayRegistrationDetails = source.DisplayRegistrationDetails
            };

            return viewModel;
        }

        public CheckAnswersViewModel Map(SubmissionsYearDetails source)
        {
            // Uses the current year if no explicit year is provided
            int year = source.Year ?? source.SmallProducerSubmissionData.CurrentSubmission.ComplianceYear;
            return Map(source, year);
        }
    }
}