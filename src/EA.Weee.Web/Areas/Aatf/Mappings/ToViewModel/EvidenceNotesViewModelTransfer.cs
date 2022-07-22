namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core;

    public class EvidenceNotesViewModelTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public Guid AatfId { get; protected set; }

        public EvidenceNoteSearchDataResult NoteData { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; private set; }

        public EvidenceNotesViewModelTransfer(Guid organisationId, 
            Guid aatfId, EvidenceNoteSearchDataResult noteData, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => noteData, noteData);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            OrganisationId = organisationId;
            AatfId = aatfId;
            NoteData = noteData;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}