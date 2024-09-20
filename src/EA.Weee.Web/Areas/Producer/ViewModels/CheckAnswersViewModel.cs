namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations;
    using System;

    public class CheckAnswersViewModel : IProducerSubmissionViewModel
    {
        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }

        public EditOrganisationDetailsViewModel OrganisationDetails { get; set; }
        public EditContactDetailsViewModel ContactDetails { get; set; }
        public ServiceOfNoticeViewModel ServiceOfNoticeData { get; set; }
        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetails { get; set; }
        public EditEeeDataViewModel EeeData { get; set; }
        public bool? RedirectToCheckAnswers { get; set; }
    }
}