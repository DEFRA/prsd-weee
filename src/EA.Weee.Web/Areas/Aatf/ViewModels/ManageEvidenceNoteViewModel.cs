namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    public class ManageEvidenceNoteViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public string AatfName { get; set; }

        public bool SingleAatf { get; set; }

        public ManageEvidenceNoteViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EvidenceSummary)
        {
        }
    }
}