namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid organisationId, List<EvidenceNoteData> notes, string schemeName) : base(organisationId, notes, schemeName)
        {
        }
    }
}