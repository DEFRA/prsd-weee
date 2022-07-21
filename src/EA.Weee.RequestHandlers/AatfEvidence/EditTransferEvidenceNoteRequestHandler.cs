﻿namespace EA.Weee.RequestHandlers.AatfEvidence
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
    using Domain.Scheme;
    using Factories;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    public class EditTransferEvidenceNoteRequestHandler : IRequestHandler<EditTransferEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ITransferTonnagesValidator transferTonnagesValidator;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public EditTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IUserContext userContext, 
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

            Condition.Requires(request.Status)
                .IsInRange(Core.AatfEvidence.NoteStatus.Draft, Core.AatfEvidence.NoteStatus.Submitted);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();
            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var scheme = await genericDataAccess.GetById<Scheme>(request.RecipientId);
            
            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(scheme).IsNotNull();

            var note = await evidenceDataAccess.GetNoteById(request.TransferNoteId);

            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    var newNoteTonnages = request.TransferValues.Select(t => new NoteTransferTonnage(t.Id,
                        t.FirstTonnage,
                        t.SecondTonnage)).ToList();

                    await evidenceDataAccess.UpdateTransfer(note,
                        scheme.Organisation,
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