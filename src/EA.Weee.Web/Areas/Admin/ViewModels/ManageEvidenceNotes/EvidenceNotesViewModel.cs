namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;

    public class EvidenceNotesViewModel
    {
        public Guid OrganisationId { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        protected EvidenceNotesViewModel(Guid organisationId, List<EvidenceNoteData> notes, ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);

            OrganisationId = organisationId;
            Notes = notes;
        }
    }
}
