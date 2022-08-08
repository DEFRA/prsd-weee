namespace EA.Weee.DataAccess.Tests.Integration.Base
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Evidence;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;

    public abstract class EvidenceNoteBaseDataAccess
    {
        protected async Task<Note> SetupSingleNote(WeeeContext context,
            DatabaseWrapper database,
            NoteType noteType = null,
            EA.Weee.Domain.Organisation.Organisation recipientOrganisation = null,
            int? complianceYear = null)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

            context.Organisations.Add(organisation);

            var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation, year: SystemTime.UtcNow.Year);

            context.Aatfs.Add(aatf1);

            await database.WeeeContext.SaveChangesAsync();

            if (noteType == null)
            {
                noteType = NoteType.EvidenceNote;
            }

            if (recipientOrganisation == null)
            {
                recipientOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientScheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);
                database.WeeeContext.Schemes.Add(recipientScheme);

                await database.WeeeContext.SaveChangesAsync();
            }

            if (complianceYear == null)
            {
                complianceYear = SystemTime.UtcNow.Year;
            }

            var note = noteType.Equals(NoteType.EvidenceNote) ? 
                NoteCommon.CreateNote(database, organisation, recipientOrganisation, aatf1, complianceYear: complianceYear) : 
                NoteCommon.CreateTransferNote(database, organisation, recipientOrganisation, complianceYear: complianceYear);

            context.Notes.Add(note);

            await database.WeeeContext.SaveChangesAsync();
            return note;
        }

        protected async Task<List<Note>> GetExistingNotesInDb(WeeeContext context, int pageSize = int.MaxValue, int pageNumber = 1)
        {
            var notes = await context.Notes.OrderByDescending(n => n.Reference)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return notes;
        }
    }
}
