namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using System;

    public class SchemeTabViewModelMapTransfer : BaseEvidenceNotesViewModelMapTransfer
    {
        public SchemeTabViewModelMapTransfer(Guid organisationId,
            EvidenceNoteSearchDataResult noteData,
            SchemePublicInfo scheme,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber) : base(organisationId, noteData, scheme, currentDate, manageEvidenceNoteViewModel, pageNumber)
        {
        }
    }
}