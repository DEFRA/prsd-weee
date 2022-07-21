namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Requests.AatfEvidence;

    public class TransferTonnagesValidator : ITransferTonnagesValidator
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public TransferTonnagesValidator(IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
        }

        public async Task Validate(List<TransferTonnageValue> transferValues, Guid? existingTransferNoteId  = null)
        {
            var existingNoteTonnage =
                await evidenceDataAccess.GetTonnageByIds(transferValues.Select(t => t.Id).ToList());

            foreach (var noteTonnage in existingNoteTonnage)
            {
                if (!noteTonnage.Received.HasValue)
                {
                    throw new InvalidOperationException($"Note tonnage with id {noteTonnage.Id} has null received");
                }

                var totalReceivedAvailable = 
                    noteTonnage.Received.Value - (noteTonnage.NoteTransferTonnage != null && noteTonnage.NoteTransferTonnage.Any() ? 
                                                 noteTonnage.NoteTransferTonnage.Where(nt => nt.Received.HasValue && 
                                                     !nt.TransferNote.Status.Value.Equals(NoteStatus.Rejected.Value) && 
                                                     !nt.TransferNote.Status.Value.Equals(NoteStatus.Void.Value) &&
                                                     (!existingTransferNoteId.HasValue || nt.TransferNoteId != existingTransferNoteId.Value)).Sum(nt => nt.Received.Value) : 0);
                var requestedTonnage =
                    transferValues.First(t => t.Id == noteTonnage.Id);

                if (requestedTonnage.FirstTonnage.HasValue && decimal
                        .Compare(requestedTonnage.FirstTonnage.Value, totalReceivedAvailable).Equals(1))
                {
                    // can't transfer more than is available
                    throw new InvalidOperationException($"The requested tonnage {requestedTonnage.FirstTonnage.Value} for note tonnage {noteTonnage.Id} is no longer available");
                }
            }
        }
    }
}
