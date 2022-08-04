namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Factories;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    public class EditTransferEvidenceNoteRequestHandler : SaveTransferNoteRequestBase, IRequestHandler<EditTransferEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public EditTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IEvidenceDataAccess evidenceDataAccess, 
            ITransferTonnagesValidator transferTonnagesValidator, 
            IWeeeTransactionAdapter transactionAdapter, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.evidenceDataAccess = evidenceDataAccess;
            this.transferTonnagesValidator = transferTonnagesValidator;
            this.transactionAdapter = transactionAdapter;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<Guid> HandleAsync(EditTransferEvidenceNoteRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);
            var transferNote = await evidenceDataAccess.GetNoteById(request.TransferNoteId);
            if (!EnsureTheSchemeNotChanged(transferNote, request.RecipientId))
            {
                throw new InvalidOperationException($"Transfer Evidence note {transferNote.Id} has incorrect Recipient Id to be saved");
            }

            Condition.Requires(request.Status)
                .IsInRange(Core.AatfEvidence.NoteStatus.Draft, Core.AatfEvidence.NoteStatus.Submitted);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();
            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var recipientOrganisation = await genericDataAccess.GetById<Organisation>(request.RecipientId);
            
            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(recipientOrganisation).IsNotNull();

            ValidToSave(organisation, transferNote.ComplianceYear, currentDate);

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    var newNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.Id,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    await evidenceDataAccess.UpdateTransfer(transferNote,
                        recipientOrganisation,
                        newNoteTonnages,
                        request.Status.ToDomainEnumeration<NoteStatus>(),
                        CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate));

                    await transferTonnagesValidator.Validate(request.TransferValues, request.TransferNoteId);

                    transactionAdapter.Commit(transaction);
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }

                return transferNote.Id;
            }
        }

        private bool EnsureTheSchemeNotChanged(Note transferNote, Guid recipientOrganisationId)
        {
            if (transferNote.Status == NoteStatus.Returned)
            {
                return transferNote.RecipientId.Equals(recipientOrganisationId);
            }
            return true;
        }
    }
}