namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;

    public abstract class ManageEvidenceNoteViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public ManageEvidenceNotesDisplayOptions ActiveDisplayOption { get; set; }
    }
}