namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core;
    using System;
    using System.Linq;
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

            var selectedIds = viewModel.CategoryValues.Where(c => c.Selected).Select(c => c.CategoryId).ToList();

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
                new TransferTonnageValue(Guid.Empty, t.CategoryId, t.Received.ToDecimal(), t.Reused.ToDecimal(),
                    t.TransferTonnageId));

            return new TransferEvidenceNoteRequest(request.OrganisationId, 
                request.SchemeId, 
                request.CategoryIds,
                transferValues.ToList(),
                request.EvidenceNoteIds,
                viewModel.Action.Equals(ActionEnum.Save) ? NoteStatus.Draft : NoteStatus.Submitted);
        }
    }
}
