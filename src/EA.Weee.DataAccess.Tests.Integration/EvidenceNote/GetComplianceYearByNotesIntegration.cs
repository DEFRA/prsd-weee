namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Base;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetComplianceYearByNotesIntegration : EvidenceNoteBaseDataAccess
    {
        [Fact]
        public async Task GetComplianceYearByNotes_GivenNotesShouldReturnCorrectComplianceYear()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var note1 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now);
                var note2 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now.AddYears(1));
                var note3 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now.AddYears(2));

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var year = await dataAccess.GetComplianceYearByNotes(new List<Guid>() { note1.Id, note2.Id, note3.Id });

                year.Should().Be((short)DateTime.Now.Year);
            }
        }

        [Fact]
        public async Task GetComplianceYearByNotes_GivenNotesWhereEarliestNoteIsNotProvidedShouldReturnCorrectComplianceYear()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var note1 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now);
                var note2 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now.AddYears(1));
                var note3 = NoteCommon.CreateNote(database, organisation1, startDate: DateTime.Now.AddYears(2));

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var year = await dataAccess.GetComplianceYearByNotes(new List<Guid>() { note2.Id, note3.Id });

                year.Should().Be((short)DateTime.Now.AddYears(1).Year);
            }
        }
    }
}
