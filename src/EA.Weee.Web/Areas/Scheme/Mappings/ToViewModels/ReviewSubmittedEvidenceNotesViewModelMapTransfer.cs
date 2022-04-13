namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public string OrganisationName { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid organisationId, List<EvidenceNoteData> notes, string organisationName)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);
            Guard.ArgumentNotNull(() => organisationName, organisationName);

            OrganisationId = organisationId;
            Notes = notes;
            OrganisationName = organisationName;
        }
    }
}