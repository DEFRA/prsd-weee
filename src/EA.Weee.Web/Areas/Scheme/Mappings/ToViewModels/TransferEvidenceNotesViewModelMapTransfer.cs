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
        public TransferEvidenceTonnageViewModel ExistingTransferTonnageViewModel { get; set; }

        public TransferEvidenceNoteCategoriesViewModel ExistingTransferEvidenceNoteCategoriesViewModel { get; }

        public EvidenceNoteSearchDataResult Notes { get; }

        public TransferEvidenceNoteRequest Request { get; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; }

        public IList<OrganisationSchemeData> RecipientData { get; set; }

        public IList<int> Categories { get; }

        public Guid OrganisationId { get; }

        public bool TransferAllTonnage { get; set; }

        public IList<Guid> SessionEvidenceNotesId { get; set; }

        public int ComplianceYear { get; set; }

        public bool? ReturnToEditDraftTransfer { get; set; }

        public List<Guid> ExcludeEvidenceNoteIds { get; set; }

        public int PageNumber { get; private set; } = 1;

        public int PageSize { get; private set; } = 10;

        public TransferEvidenceNotesViewModelMapTransfer(
            EvidenceNoteSearchDataResult notes,
            TransferEvidenceNoteRequest request,
            TransferEvidenceNoteData transferNoteData,
            Guid organisationId, 
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Request = request;
            Notes = notes;
            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public TransferEvidenceNotesViewModelMapTransfer(
            int complianceYear,
            EvidenceNoteSearchDataResult notes,
            TransferEvidenceNoteRequest request,
            Guid organisationId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(request).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Notes = notes;
            Request = request;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public TransferEvidenceNotesViewModelMapTransfer(TransferEvidenceNoteData transferNoteData,
            IList<OrganisationSchemeData> recipientData,
            Guid organisationId,
            TransferEvidenceNoteCategoriesViewModel existingModel)
        {
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(recipientData).IsNotNull();

            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
            RecipientData = recipientData;
            ExistingTransferEvidenceNoteCategoriesViewModel = existingModel;
        }
    }
}