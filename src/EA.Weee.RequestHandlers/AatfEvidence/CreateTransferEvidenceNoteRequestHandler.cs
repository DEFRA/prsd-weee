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

        public CreateTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IUserContext userContext, 
            IEvidenceDataAccess evidenceDataAccess, 
            ITransferTonnagesValidator transferTonnagesValidator, 
            IWeeeTransactionAdapter transactionAdapter)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
            this.evidenceDataAccess = evidenceDataAccess;
            this.transferTonnagesValidator = transferTonnagesValidator;
            this.transactionAdapter = transactionAdapter;
        }

        public async Task<Guid> HandleAsync(TransferEvidenceNoteRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            Condition.Requires(request.Status)
                .IsInRange(Core.AatfEvidence.NoteStatus.Draft, Core.AatfEvidence.NoteStatus.Submitted);

            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var scheme = await genericDataAccess.GetById<Scheme>(request.SchemeId);

            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(scheme).IsNotNull();

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                Guid transferNoteId;
                try
                {
                    //Validation will be put back in when only available tonnages are returned to the front end. 
                    //currently this will restrict testing too much
                    //await transferTonnagesValidator.Validate(request.TransferValues);

                    var transferNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.TransferTonnageId,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    var transferCategories = request.CategoryIds.Select(t =>
                        new NoteTransferCategory((WeeeCategory)t)).ToList();

                    transferNoteId = await evidenceDataAccess.AddTransferNote(organisation, scheme, transferCategories,
                        transferNoteTonnages, request.Status.ToDomainEnumeration<NoteStatus>(),
                        userContext.UserId.ToString());
                }
                catch
                {
                    transactionAdapter.Rollback(transaction);
                    throw;
                }

                transactionAdapter.Commit(transaction);

                return transferNoteId;
            }
        }
    }
}