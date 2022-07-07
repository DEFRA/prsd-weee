namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using ViewModels;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNotesViewModelMapTransfer
    {
        public TransferEvidenceNoteCategoriesViewModel ExistingTransferEvidenceNoteCategoriesViewModel { get; }

        public IList<EvidenceNoteData> Notes { get; }

        public TransferEvidenceNoteRequest Request { get; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; }

        public IList<SchemeData> SchemeData { get; set; }

        public IList<int> Categories { get; }

        public Guid OrganisationId { get; }

        public bool TransferAllTonnage { get; set; }

        public TransferEvidenceNotesViewModelMapTransfer(IList<EvidenceNoteData> notes,
            TransferEvidenceNoteData transferNoteData,
            Guid organisationId)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Notes = notes;
            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
        }

        public TransferEvidenceNotesViewModelMapTransfer(IList<EvidenceNoteData> notes,
            TransferEvidenceNoteRequest request,
            Guid organisationId)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(request).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Notes = notes;
            Request = request;
            OrganisationId = organisationId;
        }

        public TransferEvidenceNotesViewModelMapTransfer(TransferEvidenceNoteData transferNoteData,
            IList<SchemeData> schemeData,
            Guid organisationId,
            TransferEvidenceNoteCategoriesViewModel existingModel)
        {
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(schemeData).IsNotNull();

            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
            SchemeData = schemeData;
            ExistingTransferEvidenceNoteCategoriesViewModel = existingModel;
        }
    }
}