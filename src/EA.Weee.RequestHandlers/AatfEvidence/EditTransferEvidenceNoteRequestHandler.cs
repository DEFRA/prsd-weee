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
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    public class EditTransferEvidenceNoteRequestHandler : SaveNoteRequestBase, IRequestHandler<EditTransferEvidenceNoteRequest, Guid>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IWeeeTransactionAdapter transactionAdapter;

        public EditTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IEvidenceDataAccess evidenceDataAccess, 
            ITransferTonnagesValidator transferTonnagesValidator, 
            IWeeeTransactionAdapter transactionAdapter, 
            ISystemDataDataAccess systemDataDataAccess, WeeeContext context, IUserContext userContext) : base(context, userContext, authorization, systemDataDataAccess)
        {
            this.genericDataAccess = genericDataAccess;
            this.evidenceDataAccess = evidenceDataAccess;
            this.transferTonnagesValidator = transferTonnagesValidator;
            this.transactionAdapter = transactionAdapter;
        }

        public async Task<Guid> HandleAsync(EditTransferEvidenceNoteRequest request)
        {
            Authorization.EnsureCanAccessExternalArea();
            Authorization.EnsureOrganisationAccess(request.OrganisationId);

            Condition.Requires(request.Status)
                .IsInRange(Core.AatfEvidence.NoteStatus.Draft, Core.AatfEvidence.NoteStatus.Submitted);

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();
            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var recipientOrganisation = await genericDataAccess.GetById<Organisation>(request.RecipientId);
            
            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(recipientOrganisation).IsNotNull();

            var note = await evidenceDataAccess.GetNoteById(request.TransferNoteId);

            ValidToSave(organisation, note.ComplianceYear, currentDate);

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    var newNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.Id,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    await evidenceDataAccess.UpdateTransfer(note,
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

                return note.Id;
            }
        }
    }
}