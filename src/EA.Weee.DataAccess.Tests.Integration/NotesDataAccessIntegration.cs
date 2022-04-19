namespace EA.Weee.DataAccess.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Evidence;
    using EA.Weee.Core.Tests.Unit.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    public class NotesDataAccessIntegration
    {
        [Fact]
        public async Task GetAllNotes_ShouldMatchOnOrganisationId()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

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

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation1.Id,
                    AatfId = aatf1.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note1.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnRequiredDraftStatus()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                
                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);
                
                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation1.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(draftNote.Id);
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Submitted));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Approved));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Rejected));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Void));
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnRequiredSubmittedStatus()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());

                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation1.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Submitted }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(submittedNote.Id);
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Draft));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Approved));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Rejected));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Void));
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnRequiredSubmittedAndDraftStatus()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());

                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation1.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft, NoteStatus.Submitted }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(2);
                notes.Should().Contain(n => n.Id.Equals(draftNote.Id));
                notes.Should().Contain(n => n.Id.Equals(submittedNote.Id));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Approved));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Rejected));
                notes.Should().NotContain(n => n.Status.Equals(NoteStatus.Void));
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnAatfId()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

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

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation1.Id,
                    AatfId = aatf2.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

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

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(3);
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenDraftStatusShouldNotIncludedSubmitted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation);

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                context.Aatfs.Add(aatf);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation, null, aatf);
                var note2 = NoteCommon.CreateNote(database, organisation, null, aatf);
                note2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());

                var note3 = NoteCommon.CreateNote(database, organisation, null, aatf);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter()
                {
                    OrganisationId = organisation.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(2);
                notes.Should().NotContain(n => n.Id.Equals(note2.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenSearchRefShouldReturnSingleNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var noteShouldBeFound = await SetupSingleNote(context, database);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter()
                {
                    SearchRef = noteShouldBeFound.Reference.ToString(),
                    AllowedStatuses = new List<NoteStatus>() { noteShouldBeFound.Status}
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenSearchRefAlongWithOrganisationAndAatfShouldReturnSingleNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var noteShouldBeFound = await SetupSingleNote(context, database);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter()
                {
                    SearchRef = noteShouldBeFound.Reference.ToString(),
                    AllowedStatuses = new List<NoteStatus>() { noteShouldBeFound.Status },
                    OrganisationId = noteShouldBeFound.OrganisationId,
                    AatfId = noteShouldBeFound.AatfId
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenSearchRefWithInvalidNoteTypeShouldNotReturnNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter()
                {
                    SearchRef = $"Z{noteShouldNotBeFound.Reference}",
                    AllowedStatuses = new List<NoteStatus>() { noteShouldNotBeFound.Status },
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
            }
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public async Task GetAllNotes_GivenSearchRefWithNoteTypeAlongWithOrganisationAndAatfShouldReturnSingleNote(NoteType noteType)
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var noteShouldBeFound = await SetupSingleNote(context, database, noteType);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter()
                {
                    SearchRef = $"{noteType.DisplayName}{noteShouldBeFound.Reference}",
                    AllowedStatuses = new List<NoteStatus>() { noteShouldBeFound.Status },
                    OrganisationId = noteShouldBeFound.OrganisationId,
                    AatfId = noteShouldBeFound.AatfId
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenSearchRefWithNoteTypeAndNoteTypeDoesNotMatch_ShouldNotReturnNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var noteShouldNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote);

                var filter = new EvidenceNoteFilter()
                {
                    SearchRef = $"{NoteType.TransferNote.DisplayName}{noteShouldNotBeFound.Reference}",
                    AllowedStatuses = new List<NoteStatus>() { noteShouldNotBeFound.Status },
                    OrganisationId = noteShouldNotBeFound.OrganisationId,
                    AatfId = noteShouldNotBeFound.AatfId
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
            }
        }

        private async Task<Note> SetupSingleNote(WeeeContext context, DatabaseWrapper database, NoteType noteType = null)
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

            var note1 = NoteCommon.CreateNote(database, organisation, null, aatf1, noteType: noteType);

            context.Notes.Add(note1);

            await database.WeeeContext.SaveChangesAsync();
            return note1;
        }
    }
}
