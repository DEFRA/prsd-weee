namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class ViewAndTransferEvidenceViewModelMapTransfer
    {
        public Guid OrganisationId { get; protected set; }

        public string SchemeName { get; protected set; }

        public List<EvidenceNoteData> Notes { get; protected set; }

        public ViewAndTransferEvidenceViewModelMapTransfer(Guid organisationId, List<EvidenceNoteData> notes, string schemeName)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => notes, notes);
            Guard.ArgumentNotNull(() => schemeName, schemeName);

            OrganisationId = organisationId;
            Notes = notes;
            SchemeName = schemeName;
        }
    }
}