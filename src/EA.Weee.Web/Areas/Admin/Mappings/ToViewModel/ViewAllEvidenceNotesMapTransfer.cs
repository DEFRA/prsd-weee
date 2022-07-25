namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewAllEvidenceNotesMapTransfer
    {
        public EvidenceNoteSearchDataResult NoteData { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public ViewAllEvidenceNotesMapTransfer(EvidenceNoteSearchDataResult noteData,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotNull(() => noteData, noteData);

            NoteData = noteData;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}
