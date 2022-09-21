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

        public EvidenceNoteSearchDataResult SelectedNotes { get; }

        public EvidenceNoteSearchDataResult AvailableNotes { get; }

        public TransferEvidenceNoteRequest Request { get; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; }

        public IList<OrganisationSchemeData> RecipientData { get; set; }

        public IList<int> Categories { get; }

        public Guid OrganisationId { get; }

        public bool TransferAllTonnage { get; set; }

        public IList<Guid> SessionEvidenceNotesId { get; set; }

        public int ComplianceYear { get; set; }

        public bool? ReturnToEditDraftTransfer { get; set; }

        public int PageNumber { get; private set; } = 1;

        public int PageSize { get; private set; } = 10;

        public string SearchRef { get; private set; }

        public TransferEvidenceNotesViewModelMapTransfer(
            EvidenceNoteSearchDataResult selectedNotes,
            TransferEvidenceNoteRequest request,
            TransferEvidenceNoteData transferNoteData,
            Guid organisationId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(selectedNotes).IsNotNull();
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Request = request;
            SelectedNotes = selectedNotes;
            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public TransferEvidenceNotesViewModelMapTransfer(
            EvidenceNoteSearchDataResult selectedNotes,
            EvidenceNoteSearchDataResult availableNotes,
            TransferEvidenceNoteRequest request,
            TransferEvidenceNoteData transferNoteData,
            Guid organisationId,
            string searchRef,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(selectedNotes).IsNotNull();
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(availableNotes).IsNotNull();

            Request = request;
            SelectedNotes = selectedNotes;
            AvailableNotes = availableNotes;
            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchRef = searchRef;
        }

        public TransferEvidenceNotesViewModelMapTransfer(
            int complianceYear,
            EvidenceNoteSearchDataResult selectedNotes,
            EvidenceNoteSearchDataResult availableNotes,
            TransferEvidenceNoteRequest request,
            Guid organisationId,
            string searchRef,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(selectedNotes).IsNotNull();
            Condition.Requires(availableNotes).IsNotNull();
            Condition.Requires(request).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            SelectedNotes = selectedNotes;
            AvailableNotes = availableNotes;
            Request = request;
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchRef = searchRef;
        }

        public TransferEvidenceNotesViewModelMapTransfer(
            int complianceYear,
            EvidenceNoteSearchDataResult selectedNotes,
            TransferEvidenceNoteRequest request,
            Guid organisationId, 
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(selectedNotes).IsNotNull();
            Condition.Requires(request).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            SelectedNotes = selectedNotes;
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