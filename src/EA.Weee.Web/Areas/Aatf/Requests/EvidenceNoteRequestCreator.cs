namespace EA.Weee.Web.Areas.Aatf.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Extensions;
    using Prsd.Core;
    using ViewModels;
    using Web.Requests.Base;
    using Web.ViewModels.Shared;
    using Weee.Requests.Aatf;

    public abstract class EvidenceNoteRequestCreator<T> : IRequestCreator<EditEvidenceNoteViewModel, T> where T : new()
    {
        public virtual T ViewModelToRequest(EditEvidenceNoteViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);

            var tonnageValues = new List<TonnageValues>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                var received = categoryValue.Received.ToDecimal();
                var reused = categoryValue.Reused.ToDecimal();

                tonnageValues.Add(new TonnageValues(categoryValue.Id, categoryValue.CategoryId, received, reused));
            }

            if (viewModel.RecipientId != null)
            {
                var request = (T)Activator.CreateInstance(typeof(T), viewModel.OrganisationId,
                    viewModel.AatfId,
                    viewModel.RecipientId.Value,
                    viewModel.StartDate,
                    viewModel.EndDate,
                    viewModel.WasteTypeValue,
                    viewModel.ProtocolValue,
                    tonnageValues,
                    viewModel.Action == ActionEnum.Save ? (viewModel.Status == 0 ? NoteStatus.Draft : viewModel.Status) : NoteStatus.Submitted,
                    viewModel.Id);

                return request;
            }

            throw new InvalidOperationException("EvidenceNoteBaseRequest Received Id Should Not Be Null");
        }
    }
}