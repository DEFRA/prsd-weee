namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using System;
    using System.Linq;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using Prsd.Core;

    public abstract class TransferEvidenceNoteRequestCreator<T> : IRequestCreator<TransferSelectedDataModel, T> where T : new()
    {
        public virtual T ViewModelToRequest(TransferSelectedDataModel model)
        {
            Guard.ArgumentNotNull(() => model, model);

            if (model.SelectedSchemeId == null)
            {
                throw new InvalidOperationException("TransferEvidenceNoteRequest PCS(Schema) Id Should Be Not NULL");
            }

            if (!model.SelectedCategoryIds.Any())
            {
                throw new InvalidOperationException("TransferEvidenceNoteRequest At Least One Category Must Be Selected");
            }

            var newRequest = (T)Activator.CreateInstance(typeof(T));

            return newRequest;
        }
    }
}
