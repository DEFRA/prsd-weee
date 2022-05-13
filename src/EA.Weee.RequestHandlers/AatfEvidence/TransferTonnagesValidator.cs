namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Requests.Scheme;

    public class TransferTonnagesValidator : ITransferTonnagesValidator
    {
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public TransferTonnagesValidator(IEvidenceDataAccess evidenceDataAccess)
        {
            this.evidenceDataAccess = evidenceDataAccess;
        }

        public async Task Validate(TransferEvidenceNoteRequest request)
        {
            var existingNoteTonnage =
                await evidenceDataAccess.GetTonnageByIds(request.TransferValues.Select(t => t.TransferTonnageId)
                    .ToList());

            foreach (var noteTonnage in existingNoteTonnage)
            {
                if (!noteTonnage.Received.HasValue)
                {
                    throw new InvalidOperationException();
                }

                var totalReceivedAvailable = noteTonnage.Received.Value - noteTonnage.NoteTransferTonnage
                    .Where(nt =>
                        nt.Received.HasValue && nt.TransferNote.Status.Value.Equals(NoteStatus.Approved.Value))
                    .Sum(nt => nt.Received.Value);
                var requestedTonnage =
                    request.TransferValues.First(t => t.TransferTonnageId.Equals(noteTonnage.Id));

                if (requestedTonnage.FirstTonnage.HasValue && decimal
                        .Compare(requestedTonnage.FirstTonnage.Value, totalReceivedAvailable).Equals(1))
                {
                    // can't transfer more than is available
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
