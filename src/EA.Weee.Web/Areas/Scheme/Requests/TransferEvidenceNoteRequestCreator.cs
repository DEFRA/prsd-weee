namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using System;
    using System.Linq;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using Prsd.Core;

    public abstract class TransferEvidenceNoteRequestCreator<T> : IRequestCreator<TransferEvidenceNoteCategoriesViewModel, T> where T : new()
    {
        public virtual T ViewModelToRequest(TransferEvidenceNoteCategoriesViewModel viewModel)
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

            var newRequest = (T)Activator.CreateInstance(typeof(T), 
                viewModel.OrganisationId,
                viewModel.SelectedSchema,
                selectedIds);

            return newRequest;
        }
    }
}
