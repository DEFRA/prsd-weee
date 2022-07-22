namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using EA.Weee.Web.ViewModels.Shared;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData, 
            string schemeName, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel) : base(organisationId, noteData, schemeName, currentDate, manageEvidenceNoteViewModel)
        {
        }
    }
}