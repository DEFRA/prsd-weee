namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [Serializable]
    public class ManageEvidenceNoteViewModel 
    {
        public string AatfName { get; set; }

        public bool SingleAatf { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public FilterViewModel FilterViewModel { get; set; }

        public SubmittedDatesFilterViewModel SubmittedDatesFilterViewModel { get; set; }

        public RecipientWasteStatusFilterViewModel RecipientWasteStatusFilterViewModel { get; set; }

        public IEnumerable<int> ComplianceYearList { get; set; }

        [DisplayName("Compliance year")]
        public int ComplianceYear { get; set; }

        public ManageEvidenceNoteViewModel()
        {
            FilterViewModel = new FilterViewModel();
            SubmittedDatesFilterViewModel = new SubmittedDatesFilterViewModel();
            RecipientWasteStatusFilterViewModel = new RecipientWasteStatusFilterViewModel();
        }
    }
}