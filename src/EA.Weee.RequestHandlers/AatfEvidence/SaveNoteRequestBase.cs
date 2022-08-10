namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using Factories;
    using Prsd.Core.Domain;
    using Security;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public abstract class SaveNoteRequestBase
    {
        protected readonly IWeeeAuthorization Authorization;
        protected readonly WeeeContext Context;
        protected readonly IUserContext UserContext;
        protected readonly ISystemDataDataAccess SystemDataDataAccess;

        protected SaveNoteRequestBase(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization,
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.Authorization = authorization;
            this.SystemDataDataAccess = systemDataDataAccess;
            this.Context = context;
            this.UserContext = userContext;
        }

        private static string YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState => "You cannot manage evidence as scheme is not in a valid state";

        public void ValidToSave(Organisation organisation, int complianceYear, DateTime systemDateTime)
        {
            if (!organisation.IsBalancingScheme)
            {
                if (organisation.Scheme.SchemeStatus == SchemeStatus.Withdrawn)
                {
                    throw new InvalidOperationException(YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState);
                }
            }

            if (!WindowHelper.IsDateInComplianceYear(complianceYear, systemDateTime))
            {
                throw new InvalidOperationException(YouCannotCreateTransferEvidenceAsSchemeIsNotInAValidState);
            }
        }

        protected async Task<Guid> UpdateNoteStatus(Note note, NoteStatus status, DateTime currentDateTime, string reason = null)
        {
            var changedBy = UserContext.UserId.ToString();

            note.UpdateStatus(status.ToDomainEnumeration<Domain.Evidence.NoteStatus>(), changedBy, CurrentSystemTimeHelper.GetCurrentTimeBasedOnSystemTime(currentDateTime), reason);

            await Context.SaveChangesAsync();

            return note.Id;
        }

        protected async Task<Note> EvidenceNote(Guid noteId)
        {
            var evidenceNote = await Context.Notes.FindAsync(noteId);

            Condition.Requires(evidenceNote).IsNotNull();
            return evidenceNote;
        }
    }
}
