namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core;
    using System;
    using System.Linq;
    using Core.Aatf;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Extensions;
    using ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNoteRequestCreator : ITransferEvidenceRequestCreator
    {
        public TransferEvidenceNoteRequest SelectCategoriesToRequest(TransferEvidenceNoteCategoriesViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);

            if (viewModel.SelectedSchema == null)
            {
                throw new InvalidOperationException("TransferEvidenceNoteRequest PCS(Schema) Id Should Be Not NULL");
            }

            var selectedIds = viewModel.CategoryBooleanViewModels.Where(c => c.Selected).Select(c => c.CategoryId).ToList();

            if (!selectedIds.Any())
            {
                throw new InvalidOperationException("TransferEvidenceNoteRequest At Least One Category Id Must Be Selected");
            }

            var newRequest = new TransferEvidenceNoteRequest(
                viewModel.OrganisationId,
                viewModel.SelectedSchema.Value,
                selectedIds);

            return newRequest;
        }

        public TransferEvidenceNoteRequest SelectTonnageToRequest(TransferEvidenceNoteRequest request, TransferEvidenceTonnageViewModel viewModel)
        {
            Condition.Requires(request).IsNotNull();
            Condition.Requires(viewModel).IsNotNull();

            var transferValues = viewModel.TransferCategoryValues.Select(t =>
                new TransferTonnageValue(t.Id, t.CategoryId, t.Received.ToDecimal(), t.Reused.ToDecimal(), Guid.Empty));

            return new TransferEvidenceNoteRequest(request.OrganisationId, 
                request.RecipientId, 
                request.CategoryIds,
                transferValues.ToList(),
                request.EvidenceNoteIds,
                viewModel.Action.Equals(ActionEnum.Save) ? NoteStatus.Draft : NoteStatus.Submitted);
        }

        public EditTransferEvidenceNoteRequest EditSelectTonnageToRequest(TransferEvidenceNoteRequest request, TransferEvidenceTonnageViewModel viewModel)
        {
            Condition.Requires(viewModel).IsNotNull();

            var transferValues = viewModel.TransferCategoryValues.Select(t =>
                new TransferTonnageValue(t.Id, t.CategoryId, t.Received.ToDecimal(), t.Reused.ToDecimal(),
                    t.TransferTonnageId));

            var organisationId = viewModel.PcsId;
            var recipientId = request?.RecipientId ?? viewModel.RecipientId;

            return new EditTransferEvidenceNoteRequest(viewModel.ViewTransferNoteViewModel.EvidenceNoteId,
                organisationId,
                recipientId,
                transferValues.ToList(),
                viewModel.Action.Equals(ActionEnum.Save) ? NoteStatus.Draft : NoteStatus.Submitted);
        }
    }
}
