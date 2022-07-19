namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfReturn;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using Factories;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    public class CreateTransferEvidenceNoteRequestHandler : IRequestHandler<TransferEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public CreateTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IUserContext userContext, 
            IEvidenceDataAccess evidenceDataAccess, 
            ITransferTonnagesValidator transferTonnagesValidator, 
            IWeeeTransactionAdapter transactionAdapter, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
            this.evidenceDataAccess = evidenceDataAccess;
            this.transferTonnagesValidator = transferTonnagesValidator;
            this.transactionAdapter = transactionAdapter;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<Guid> HandleAsync(TransferEvidenceNoteRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            Condition.Requires(request.Status)
                .IsInRange(Core.AatfEvidence.NoteStatus.Draft, Core.AatfEvidence.NoteStatus.Submitted);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();
            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var scheme = await genericDataAccess.GetById<Scheme>(request.RecipientId);

            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(scheme).IsNotNull();

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                Guid transferNoteId;
                try
                {
                    await transferTonnagesValidator.Validate(request.TransferValues);

                    var transferNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.Id,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    var complianceYear = await evidenceDataAccess.GetComplianceYearByNotes(request.EvidenceNoteIds);

                    transferNoteId = await evidenceDataAccess.AddTransferNote(organisation, scheme.Organisation,
                        transferNoteTonnages, request.Status.ToDomainEnumeration<NoteStatus>(), complianceYear,
                        userContext.UserId.ToString(), CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDate));

                    transactionAdapter.Commit(transaction);
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }

                return transferNoteId;
            }
        }
    }
}