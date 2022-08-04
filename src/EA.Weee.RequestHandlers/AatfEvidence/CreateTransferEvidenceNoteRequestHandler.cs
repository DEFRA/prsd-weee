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

    public class CreateTransferEvidenceNoteRequestHandler : SaveNoteRequestBase, IRequestHandler<TransferEvidenceNoteRequest, Guid>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IWeeeTransactionAdapter transactionAdapter;

        public CreateTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IUserContext userContext, 
            IEvidenceDataAccess evidenceDataAccess, 
            ITransferTonnagesValidator transferTonnagesValidator, 
            IWeeeTransactionAdapter transactionAdapter, 
            ISystemDataDataAccess systemDataDataAccess, WeeeContext context) : base(context, userContext, authorization, systemDataDataAccess)
        {
            this.genericDataAccess = genericDataAccess;
            this.evidenceDataAccess = evidenceDataAccess;
            this.transferTonnagesValidator = transferTonnagesValidator;
            this.transactionAdapter = transactionAdapter;
        }

        public async Task<Guid> HandleAsync(TransferEvidenceNoteRequest request)
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

            ValidToSave(organisation, request.ComplianceYear, currentDate);

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                Note note;
                try
                {
                    await transferTonnagesValidator.Validate(request.TransferValues);

                    var transferNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.Id,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    note = await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation,
                        transferNoteTonnages, request.Status.ToDomainEnumeration<NoteStatus>(), request.ComplianceYear,
                        UserContext.UserId.ToString(), CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate));

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