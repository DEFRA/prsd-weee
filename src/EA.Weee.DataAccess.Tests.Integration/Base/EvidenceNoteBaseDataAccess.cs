namespace EA.Weee.DataAccess.Tests.Integration.Base
{
    using System.Threading.Tasks;
    using Domain.Evidence;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;

    public abstract class EvidenceNoteBaseDataAccess
    {
        protected async Task<Note> SetupSingleNote(WeeeContext context,
            DatabaseWrapper database,
            NoteType noteType = null,
            EA.Weee.Domain.Scheme.Scheme scheme = null)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

            context.Organisations.Add(organisation);

            var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

            context.Aatfs.Add(aatf1);

            await database.WeeeContext.SaveChangesAsync();

            if (noteType == null)
            {
                noteType = NoteType.EvidenceNote;
            }

            if (scheme == null)
            {
                scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            }

            var note = noteType.Equals(NoteType.EvidenceNote) ? NoteCommon.CreateNote(database, organisation, scheme, aatf1) : NoteCommon.CreateTransferNote(database, organisation, scheme);

            context.Notes.Add(note);

            await database.WeeeContext.SaveChangesAsync();
            return note;
        }
    }
}
