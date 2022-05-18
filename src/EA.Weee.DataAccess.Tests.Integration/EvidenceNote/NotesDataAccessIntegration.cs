namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
    using Core.Tests.Unit.Helpers;
    using Domain.Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    public class NotesDataAccessIntegration : EvidenceNoteBaseDataAccess
    {
        [Fact]
        public async Task GetAllNotes_ShouldMatchOnOrganisationId()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

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

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                
                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);
                
                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());

                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var draftNote = NoteCommon.CreateNote(database, organisation1, null, null);
                var submittedNote = NoteCommon.CreateNote(database, organisation1, null, null);

                submittedNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());

                context.Notes.Add(draftNote);
                context.Notes.Add(submittedNote);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

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

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

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

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

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

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
                {
                    OrganisationId = organisation.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft },
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldBeFound = await SetupSingleNote(context, database);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldBeFound = await SetupSingleNote(context, database);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldBeFound = await SetupSingleNote(context, database, noteType);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote);

                var filter = new EvidenceNoteFilter((short)DateTime.Now.Year)
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

        [Fact]
        public async Task GetNoteCountByStatusAndAatf_GivenStatusAndAatf_ShouldReturnCorrectNoteCount()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                context.Aatfs.Add(aatf1);
                context.Aatfs.Add(aatf2);

                await database.WeeeContext.SaveChangesAsync();

                var approved1 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                approved1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                approved1.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                context.Notes.Add(approved1);

                //diff aatf not counted
                var approved2 = NoteCommon.CreateNote(database, organisation, null, aatf2);
                approved2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                approved2.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                context.Notes.Add(approved2);

                var submitted1 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                submitted1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                context.Notes.Add(submitted1);

                var submitted2 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                submitted2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                context.Notes.Add(submitted2);

                //diff aatf not counted
                var submitted3 = NoteCommon.CreateNote(database, organisation, null, aatf2);
                submitted3.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                context.Notes.Add(submitted3);

                var draft1 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                context.Notes.Add(draft1);

                var draft2 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                context.Notes.Add(draft2);

                var draft3 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                context.Notes.Add(draft3);

                //diff aatf not counted
                var draft4 = NoteCommon.CreateNote(database, organisation, null, aatf2);
                context.Notes.Add(draft4);

                var rejected1 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                rejected1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                rejected1.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(rejected1);

                var rejected2 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                rejected2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                rejected2.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(rejected2);

                var rejected3 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                rejected3.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                rejected3.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(rejected3);

                var rejected4 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                rejected4.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                rejected4.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(rejected4);

                //diff aatf not counted
                var rejected5 = NoteCommon.CreateNote(database, organisation, null, aatf2);
                rejected5.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                rejected5.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(rejected5);

                var voided1 = NoteCommon.CreateNote(database, organisation, null, aatf1);
                voided1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                voided1.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                context.Notes.Add(voided1);

                //diff aatf not counted
                var voided2 = NoteCommon.CreateNote(database, organisation, null, aatf2);
                voided2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                voided2.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                context.Notes.Add(voided2);

                await context.SaveChangesAsync();

                var notesAatf1 = await context.Notes.CountAsync(n => n.AatfId.Value.Equals(aatf1.Id));
                var notesAatf2 = await context.Notes.CountAsync(n => n.AatfId.Value.Equals(aatf2.Id));

                //confirm data added
                notesAatf1.Should().Be(11);
                notesAatf2.Should().Be(5);

                var approvedNotesAatf1 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Approved, aatf1.Id);
                var submittedNotesAatf1 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Submitted, aatf1.Id);
                var draftNotesAatf1 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Draft, aatf1.Id);
                var voidNotesAatf1 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Void, aatf1.Id);
                var rejectedNotesAatf1 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Rejected, aatf1.Id);

                approvedNotesAatf1.Should().Be(1);
                submittedNotesAatf1.Should().Be(2);
                draftNotesAatf1.Should().Be(3);
                voidNotesAatf1.Should().Be(1);
                rejectedNotesAatf1.Should().Be(4);

                var approvedNotesAatf2 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Approved, aatf2.Id);
                var submittedNotesAatf2 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Submitted, aatf2.Id);
                var draftNotesAatf2 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Draft, aatf2.Id);
                var voidNotesAatf2 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Void, aatf2.Id);
                var rejectedNotesAatf2 = await dataAccess.GetNoteCountByStatusAndAatf(NoteStatus.Rejected, aatf2.Id);

                approvedNotesAatf2.Should().Be(1);
                submittedNotesAatf2.Should().Be(1);
                draftNotesAatf2.Should().Be(1);
                voidNotesAatf2.Should().Be(1);
                rejectedNotesAatf2.Should().Be(1);
            }
        }
    }
}
