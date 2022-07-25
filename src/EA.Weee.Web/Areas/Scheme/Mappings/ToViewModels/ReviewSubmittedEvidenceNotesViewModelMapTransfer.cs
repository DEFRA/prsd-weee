namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData,
            SchemePublicInfo scheme, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel) : base(organisationId, noteData, scheme, currentDate, manageEvidenceNoteViewModel)
        {
        }
    }
}