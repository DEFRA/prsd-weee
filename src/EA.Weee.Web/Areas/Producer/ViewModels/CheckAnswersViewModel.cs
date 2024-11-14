namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using System;

    public class CheckAnswersViewModel : IProducerSubmissionViewModel
    {
        public Guid DirectRegistrantId { get; set; }

        public Guid OrganisationId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }

        public OrganisationViewModel OrganisationDetails { get; set; }
        public ContactDetailsViewModel ContactDetails { get; set; }
        public ServiceOfNoticeViewModel ServiceOfNoticeData { get; set; }
        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetails { get; set; }
        public EditEeeDataViewModel EeeData { get; set; }
        public bool? RedirectToCheckAnswers { get; set; }
        public bool? IsPdfDownload { get; set; } = false;
        public int ComplianceYear { get; set; }
    }
}