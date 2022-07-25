namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using EA.Weee.Web.ViewModels.Shared;

    public abstract class BaseEvidenceNotesViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public string SchemeName { get; protected set; }

        public EvidenceNoteSearchDataResult NoteData { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        protected BaseEvidenceNotesViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData, 
            string schemeName,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => noteData, noteData);
            Guard.ArgumentNotNull(() => schemeName, schemeName);

            OrganisationId = organisationId;
            NoteData = noteData;
            SchemeName = schemeName;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}