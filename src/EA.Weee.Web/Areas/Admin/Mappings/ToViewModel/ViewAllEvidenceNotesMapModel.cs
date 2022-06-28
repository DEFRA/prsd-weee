namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewAllEvidenceNotesMapModel
    {
        public List<AdminEvidenceNoteData> Notes { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public ViewAllEvidenceNotesMapModel(List<AdminEvidenceNoteData> notes,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotNull(() => notes, notes);

            Notes = notes;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}
