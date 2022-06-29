namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using EA.Weee.Web.ViewModels.Shared;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid organisationId, 
            List<EvidenceNoteData> notes, 
            string schemeName, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel) : base(organisationId, notes, schemeName, currentDate, manageEvidenceNoteViewModel)
        {
        }
    }
}