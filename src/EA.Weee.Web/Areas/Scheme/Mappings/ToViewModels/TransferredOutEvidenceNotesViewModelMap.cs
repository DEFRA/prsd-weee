namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using Aatf.ViewModels;

    public class TransferredOutEvidenceNotesViewModelMap : BaseEvidenceNotesViewModelMapTransfer
    {
        public TransferredOutEvidenceNotesViewModelMap(Guid organisationId,
            List<EvidenceNoteData> notes,
            string schemeName,
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel) : base(organisationId, notes, schemeName, currentDate, manageEvidenceNoteViewModel)
        {
        }
    }
}