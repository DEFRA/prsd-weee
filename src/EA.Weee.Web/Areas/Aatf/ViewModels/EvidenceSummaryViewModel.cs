namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Web.ViewModels.Shared;

    public class ManageEvidenceSummaryViewModel : ManageManageEvidenceNoteOverviewViewModel
    {
        public IList<EvidenceCategoryValue> CategoryValues { get; set; }

        public string NumberOfDraftNotes { get; set; }

        public string NumberOfSubmittedNotes { get; set; }

        public string NumberOfApprovedNotes { get; set; }

        public string TotalReceivedEvidence { get; set; }

        public string TotalReuseEvidence { get; set; }

        public ManageEvidenceSummaryViewModel()
        : base(ManageEvidenceOverviewDisplayOption.EvidenceSummary)
        {
        }
    }
}
