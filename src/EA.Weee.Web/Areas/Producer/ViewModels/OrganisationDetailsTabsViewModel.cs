namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using iText.Layout.Element;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationDetailsTabsViewModel
    {
        [DisplayName("Select registration year")]
        [Required]
        public int? Year { get; set; } = null;

        public IEnumerable<int> Years { get; set; }

        public bool IsInternal { get; set; } = false;

        public bool IsAdmin { get; set; } = false;

        public bool IsInternalAdmin => this.IsAdmin && this.IsInternal;

        public bool IsRegistered => this.Status == SubmissionStatus.Submitted && this.HasPaid;

        public bool ShowReturnRegistrationToUser => this.IsInternalAdmin && (IsRegistered || this.Status == SubmissionStatus.Submitted);

        public bool ShowContinueRegistrationToUser => !this.IsInternalAdmin && this.Status == SubmissionStatus.InComplete && this.CurrentYear == this.Year;
        
        public int? CurrentYear { get; set; }

        public bool ShowPaymentLink => this.IsInternalAdmin && this.Status == SubmissionStatus.Submitted && HasPaid == false;

        public string RegistrationNumber { get; set; }

        public OrganisationDetailsDisplayOption ActiveOption { get; set; }

        public bool HasPaid { get; set; } = false;

        public SubmissionStatus Status { get; set; }

        public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

        public OrganisationViewModel OrganisationViewModel { get; set; }

        public ContactDetailsViewModel ContactDetailsViewModel { get; set; }

        public ServiceOfNoticeViewModel ServiceOfNoticeViewModel { get; set; }

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; set; }

        public EditEeeDataViewModel EditEeeDataViewModel { get; set; }
    }
}