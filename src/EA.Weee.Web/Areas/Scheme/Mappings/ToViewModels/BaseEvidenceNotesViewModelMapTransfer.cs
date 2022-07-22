namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;

    public abstract class BaseEvidenceNotesViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public SchemePublicInfo Scheme { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        protected BaseEvidenceNotesViewModelMapTransfer(Guid organisationId, 
            List<EvidenceNoteData> notes,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);
            Guard.ArgumentNotNull(() => scheme, scheme);

            OrganisationId = organisationId;
            Notes = notes;
            Scheme = scheme;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}