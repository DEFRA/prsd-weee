namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
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

        public bool IsInternalAdmin
        {
            get
            {
                return this.IsAdmin && this.IsInternal;
            }
        }

        public bool IsRegistered
        {
            get
            {
                return this.OrganisationViewModel.Status == EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted && this.OrganisationViewModel.HasPaid;
            }
        }

        public bool ShowReturnRegistrationToUser
        {
            get
            {
                return this.IsInternalAdmin && (IsRegistered || this.OrganisationViewModel.Status == EA.Weee.Core.DirectRegistrant.SubmissionStatus.Submitted);
            }
        }

        public string RegistrationNumber { get; set; }

        public OrganisationDetailsDisplayOption ActiveOption { get; set; }

        public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

        public OrganisationViewModel OrganisationViewModel { get; set; }

        public ContactDetailsViewModel ContactDetailsViewModel { get; set; }

        public ServiceOfNoticeViewModel ServiceOfNoticeViewModel { get; set; }

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; set; }

        public EditEeeDataViewModel EditEeeDataViewModel { get; set; }
    }
}