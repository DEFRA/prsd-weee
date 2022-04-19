namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class ViewAndTransferEvidenceViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ViewAndTransferEvidenceViewModelMapTransfer(Guid organisationId, List<EvidenceNoteData> notes, string schemeName) : base(organisationId, notes, schemeName)
        {
        }
    }
}