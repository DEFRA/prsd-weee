namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewAndTransferEvidenceViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ViewAndTransferEvidenceViewModelMapTransfer(Guid organisationId, 
            List<EvidenceNoteData> notes, 
            string schemeName,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel) : base(organisationId, notes, schemeName, currentDate, manageEvidenceNoteViewModel)
        {
        }
    }
}