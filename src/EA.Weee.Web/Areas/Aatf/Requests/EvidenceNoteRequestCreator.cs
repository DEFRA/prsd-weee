namespace EA.Weee.Web.Areas.Aatf.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core.AatfEvidence;
    using Extensions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;

    public class EvidenceNoteRequestCreator : IRequestCreator<EvidenceNoteViewModel, EvidenceNoteBaseRequest>
    {
        public EvidenceNoteBaseRequest ViewModelToRequest(EvidenceNoteViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);

            var tonnageValues = new List<TonnageValues>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                var received = categoryValue.Received.ToDecimal();
                var reused = categoryValue.Reused.ToDecimal();

                tonnageValues.Add(new TonnageValues(categoryValue.Id, categoryValue.CategoryId, received, reused));
            }

            if (viewModel.ReceivedId != null)
            {
                var request = new CreateEvidenceNoteRequest(viewModel.OrganisationId,
                    viewModel.AatfId,
                    viewModel.ReceivedId.Value,
                    viewModel.StartDate,
                    viewModel.EndDate,
                    viewModel.WasteTypeValue,
                    viewModel.ProtocolValue,
                    tonnageValues,
                    viewModel.Action == ActionEnum.Save ? NoteStatus.Draft : NoteStatus.Submitted);

                return request;
            }

            throw new InvalidOperationException("EvidenceNoteBaseRequest Received Id Should Not Be Null");
        }
    }
}