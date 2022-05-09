namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
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

        [Fact]
        public async Task GetNoteCountByStatusAndAatf_GivenStatusAndAatf_ShouldReturnCorrectNoteCount()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

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

                var notesAatf1 = await context.Notes.CountAsync(n => n.AatfId.Equals(aatf1.Id));
                var notesAatf2 = await context.Notes.CountAsync(n => n.AatfId.Equals(aatf2.Id));

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

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategories_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                // to be found matching category, scheme and status
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, null));

                context.Notes.Add(note1ToBeFound);

                // note not to be found has category but with not tonnage, matching scheme and status
                var note2ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note2ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note2ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note2ToNotBeFound);

                // note not to be found not matching scheme, matching status and category
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
                var note3ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme2);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note3ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note3ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note3ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note4ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note4ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note4ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note4ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note4ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note5ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note5ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note5ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note5ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note6ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note6ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note6ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note7ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note7ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note7ToNotBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                note7ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note7ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note8ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note8ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                note8ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note8ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note9ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note9ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                note9ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, null));

                context.Notes.Add(note9ToNotBeFound);

                // to be found matching category, scheme and status
                var note10ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note10ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note10ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 1, null));

                context.Notes.Add(note10ToBeFound);

                // to not be found matching category but received is null and reused is not null, also with matching scheme and status
                var note11ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note11ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note11ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note11ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, null, 1));

                context.Notes.Add(note11ToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt(),
                    WeeeCategory.MedicalDevices.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>());

                notes.Count().Should().Be(2);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().Contain(n => n.Id.Equals(note10ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note2ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note4ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note5ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note6ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note7ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note8ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note9ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note11ToNotBeFound.Id));
            }
        }

        private async Task<Note> SetupSingleNote(WeeeContext context, 
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

            var note1 = NoteCommon.CreateNote(database, organisation, scheme, aatf1, noteType: noteType);

            context.Notes.Add(note1);

            await database.WeeeContext.SaveChangesAsync();
            return note1;
        }
    }
}
