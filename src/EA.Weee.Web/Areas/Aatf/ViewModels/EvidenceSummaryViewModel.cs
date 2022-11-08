namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Core.AatfEvidence;

    public class ManageEvidenceSummaryViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public IList<EvidenceCategoryValue> CategoryValues { get; set; }

        public string NumberOfDraftNotes { get; set; }

        public string NumberOfSubmittedNotes { get; set; }

        public string TotalReceivedEvidence { get; set; }

        public string TotalReuseEvidence { get; set; }

        public string NumberOfReturnedNotes { get; set; }

        public ManageEvidenceSummaryViewModel()
        : base(ManageEvidenceOverviewDisplayOption.EvidenceSummary)
        {
        }
    }
}
