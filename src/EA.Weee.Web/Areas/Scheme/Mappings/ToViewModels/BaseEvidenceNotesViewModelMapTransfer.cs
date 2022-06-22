namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using Aatf.ViewModels;
    using EA.Weee.Web.ViewModels.Shared;

    public abstract class BaseEvidenceNotesViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public string SchemeName { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public DateTime CurrentDate { get; protected set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; }

        protected BaseEvidenceNotesViewModelMapTransfer(Guid organisationId, 
            List<EvidenceNoteData> notes, 
            string schemeName,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);
            Guard.ArgumentNotNull(() => schemeName, schemeName);

            OrganisationId = organisationId;
            Notes = notes;
            SchemeName = schemeName;
            CurrentDate = currentDate;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
        }
    }
}