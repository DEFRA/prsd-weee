namespace EA.Weee.DataAccess.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using Core.Helpers;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Factories;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class AatfNotesDataAccessIntegration
    {
        [Fact]
        public async Task GetAllNotes_ShouldMatchOnOrganisationId()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new AatfDataAccess(database.WeeeContext, new GenericDataAccess(database.WeeeContext),
                    new QuarterWindowFactory(new QuarterWindowTemplateDataAccess(database.WeeeContext)));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);
                context.Organisations.Add(organisation2);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1);
                
                context.Aatfs.Add(aatf1);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation1, null, aatf1);
                var note2 = NoteCommon.CreateNote(database, organisation2, null, aatf1);
                var note3 = NoteCommon.CreateNote(database, organisation2, null, aatf1);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var notes = await dataAccess.GetAllNotes(organisation1.Id, aatf1.Id,
                    new List<int>() { NoteStatus.Draft.ToInt() });

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note1.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnAatfId()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new AatfDataAccess(database.WeeeContext, new GenericDataAccess(database.WeeeContext),
                    new QuarterWindowFactory(new QuarterWindowTemplateDataAccess(database.WeeeContext)));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1);

                context.Aatfs.Add(aatf1);
                context.Aatfs.Add(aatf2);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation1, null, aatf2);
                var note2 = NoteCommon.CreateNote(database, organisation1, null, aatf1);
                var note3 = NoteCommon.CreateNote(database, organisation1, null, aatf1);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var notes = await dataAccess.GetAllNotes(organisation1.Id, aatf2.Id,
                    new List<int>() { NoteStatus.Draft.ToInt() });

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note1.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldBeOrderedByCreatedDateDesc()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new AatfDataAccess(database.WeeeContext, new GenericDataAccess(database.WeeeContext),
                    new QuarterWindowFactory(new QuarterWindowTemplateDataAccess(database.WeeeContext)));

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                
                context.Organisations.Add(organisation);

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                context.Aatfs.Add(aatf);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation, null, aatf);
                var note2 = NoteCommon.CreateNote(database, organisation, null, aatf);
                var note3 = NoteCommon.CreateNote(database, organisation, null, aatf);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var notes = await dataAccess.GetAllNotes(organisation.Id, aatf.Id,
                    new List<int>() { NoteStatus.Draft.ToInt() });

                notes.Count.Should().Be(3);
                notes.Select(n => n.CreatedDate).Should().BeInDescendingOrder();
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldNotIncludedSubmitted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new AatfDataAccess(database.WeeeContext, new GenericDataAccess(database.WeeeContext),
                    new QuarterWindowFactory(new QuarterWindowTemplateDataAccess(database.WeeeContext)));

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation);

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                context.Aatfs.Add(aatf);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation, null, aatf);
                var note2 = NoteCommon.CreateNote(database, organisation, null, aatf);
                note2.UpdateStatus(Domain.Evidence.NoteStatus.Submitted, context.GetCurrentUser());

                var note3 = NoteCommon.CreateNote(database, organisation, null, aatf);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var notes = await dataAccess.GetAllNotes(organisation.Id, aatf.Id,
                    new List<int>() { NoteStatus.Draft.ToInt() });

                notes.Count.Should().Be(2);
                notes.Should().NotContain(n => n.Id.Equals(note2.Id));
            }
        }
    }
}
