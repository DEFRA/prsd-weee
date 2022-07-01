namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewAllEvidenceNotesMapModel
    {
        public List<EvidenceNoteData> Notes { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public ViewAllEvidenceNotesMapModel(List<EvidenceNoteData> notes,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotNull(() => notes, notes);

            Notes = notes;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}
