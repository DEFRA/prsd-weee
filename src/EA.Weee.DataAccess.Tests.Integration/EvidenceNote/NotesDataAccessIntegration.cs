namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
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
        public async Task GetNoteById_ShouldRetrieveMatchingNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var note1 = NoteCommon.CreateNote(database);
                
                context.Notes.Add(note1);
                
                await database.WeeeContext.SaveChangesAsync();
                
                var note = await dataAccess.GetNoteById(note1.Id);

                note.Should().NotBeNull();
                note.Id.Should().Be(note1.Id);
            }
        }

        [Fact]
        public async Task GetNoteById_ShouldNotRetrieveNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var note1 = NoteCommon.CreateNote(database);

                context.Notes.Add(note1);

                await database.WeeeContext.SaveChangesAsync();

                var exception = await Record.ExceptionAsync(async () => await dataAccess.GetNoteById(Guid.NewGuid()));

                exception.Should().BeOfType<ArgumentNullException>();
            }
        }

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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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
        public async Task GetAllNotes_GivenSchemeQuery_ShouldOnlyRetrieveEvidenceNotes()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                context.Organisations.Add(organisation1);
                
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1);

                context.Aatfs.Add(aatf1);

                await database.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, organisation1, scheme, aatf1);
                
                context.Notes.Add(note1);

                var transferNote = NoteCommon.CreateTransferNote(database, organisation1, scheme);

                context.Notes.Add(transferNote);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    OrganisationId = organisation1.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft },
                    SchemeId = scheme.Id
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note1.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnComplianceYear()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1, (short)DateTime.Now.Year);

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var note1Excluded = NoteCommon.CreateNote(database, organisation1, aatf: aatf, startDate: DateTime.Now.AddYears(1));
                var note2Included = NoteCommon.CreateNote(database, organisation1, aatf: aatf);

                context.Notes.Add(note1Excluded); 
                context.Notes.Add(note2Included); 

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    OrganisationId = organisation1.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft },
                    ComplianceYear = note2Included.ComplianceYear
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note2Included.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchOnTransferNoteType()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1, (short)DateTime.Now.Year);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var note1Excluded = NoteCommon.CreateNote(database, organisation1, aatf: aatf, startDate: DateTime.Now.AddYears(1));
                var note2Included = NoteCommon.CreateTransferNote(database, organisation1, scheme);

                context.Notes.Add(note1Excluded);
                context.Notes.Add(note2Included);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType> { NoteType.TransferNote },
                    OrganisationId = organisation1.Id,
                    AatfId = null,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(note2Included.Id);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldNotMatchOnNoteType_ShouldReturnZeroNotes()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1, (short)DateTime.Now.Year);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var note1Excluded = NoteCommon.CreateTransferNote(database, organisation1, scheme);
                var note2Excluded = NoteCommon.CreateTransferNote(database, organisation1, scheme);
                var note3Excluded = NoteCommon.CreateTransferNote(database, organisation1, scheme);

                context.Notes.Add(note1Excluded);
                context.Notes.Add(note2Excluded);
                context.Notes.Add(note3Excluded);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType> { NoteType.EvidenceNote },
                    OrganisationId = organisation1.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task GetAllNotes_ShouldMatchEvidenceNoteType_ShouldReturnAllNotes()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1, (short)DateTime.Now.Year);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                context.Organisations.Add(organisation1);

                await database.WeeeContext.SaveChangesAsync();

                var note1Included = NoteCommon.CreateNote(database, organisation1, scheme, aatf);
                var note2Included = NoteCommon.CreateNote(database, organisation1, scheme, aatf);
                var note3Included = NoteCommon.CreateNote(database, organisation1, scheme, aatf);

                context.Notes.Add(note1Included);
                context.Notes.Add(note2Included);
                context.Notes.Add(note3Included);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType> { NoteType.EvidenceNote },
                    OrganisationId = organisation1.Id,
                    AatfId = aatf.Id,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(3);
                notes.Should().Contain(n => n.Id == note1Included.Id);
                notes.Should().Contain(n => n.Id == note2Included.Id);
                notes.Should().Contain(n => n.Id == note3Included.Id);
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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
        public async Task GetAllNotes_GivenWasteType_ShouldReturnSingleNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));
               
                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                context.Aatfs.Add(aatf1);

                await database.WeeeContext.SaveChangesAsync();

                var noteShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf1, WasteType.HouseHold);
                context.Notes.Add(noteShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, null, aatf1, WasteType.NonHouseHold);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    WasteTypeId = (int?)WasteType.HouseHold,
                    AllowedStatuses = new List<NoteStatus>() { noteShouldBeFound.Status }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenRecipientFilter_ShouldReturnSingleNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                context.Organisations.Add(organisation);

                var schemeToMatch = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var schemeNotMatch = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                context.Aatfs.Add(aatf);

                await database.WeeeContext.SaveChangesAsync();

                var noteShouldBeFound = NoteCommon.CreateNote(database, organisation, schemeToMatch, aatf, WasteType.HouseHold);
                context.Notes.Add(noteShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, schemeNotMatch, aatf, WasteType.NonHouseHold);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    SchemeId = schemeToMatch.Id,
                    AllowedStatuses = new List<NoteStatus>() { noteShouldBeFound.Status }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.ElementAt(0).RecipientId.Should().Be(schemeToMatch.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenNoteStatusFilter_ShouldReturnSingleNote()
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

                var noteShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                noteShouldBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                context.Notes.Add(noteShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    NoteStatusId = NoteStatus.Void.Value,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft, NoteStatus.Approved, NoteStatus.Rejected }
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.ElementAt(0).Status.Should().Be(NoteStatus.Void);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        public static readonly object[][] StartDates =
        {
            new object[] { DateTime.Now },
            new object[] { DateTime.Now.AddDays(-1) }
        };

        [Theory, MemberData(nameof(StartDates))]
        public async Task GetAllNotes_GivenSubmittedStartDateFilter_ShouldReturnSingleNote(DateTime date)
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

                var noteShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                noteShouldBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                noteShouldBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                noteShouldBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(noteShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    StartDateSubmitted = date,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft, NoteStatus.Approved, NoteStatus.Rejected },
                    AatfId = aatf.Id
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenStartDateFilterOutSideOfStartDate_ShouldReturnZeroNote()
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

                var noteShouldNotBeFound1 = NoteCommon.CreateNote(database, organisation, null, aatf);
                noteShouldNotBeFound1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                context.Notes.Add(noteShouldNotBeFound1);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    StartDateSubmitted = DateTime.Now.AddDays(1),
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Submitted },
                    AatfId = aatf.Id
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound1.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenEndDateFilterOutSideOfEndDate_ShouldReturnZeroNote()
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

                var noteShouldNotBeFound1 = NoteCommon.CreateNote(database, organisation, null, aatf);
                noteShouldNotBeFound1.ComplianceYear = aatf.ComplianceYear;
                noteShouldNotBeFound1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                context.Notes.Add(noteShouldNotBeFound1);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(aatf.ComplianceYear)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    EndDateSubmitted = DateTime.Now.AddDays(-1),
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Submitted },
                    AatfId = aatf.Id, 
                    ComplianceYear = aatf.ComplianceYear
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound1.Id));
            }
        }

        public static readonly object[][] EndDates =
        {
            new object[] { DateTime.Now },
            new object[] { DateTime.Now.AddDays(1) }
        };

        [Theory, MemberData(nameof(EndDates))]
        public async Task GetAllNotes_GivenEndDateFilter_ShouldReturnSingleNote(DateTime date)
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

                var noteShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                noteShouldBeFound.ComplianceYear = aatf.ComplianceYear;
                noteShouldBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                noteShouldBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                noteShouldBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(noteShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(aatf.ComplianceYear)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    EndDateSubmitted = date,
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Draft, NoteStatus.Approved, NoteStatus.Rejected },
                    AatfId = aatf.Id, 
                    ComplianceYear = aatf.ComplianceYear
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(1);
                notes.ElementAt(0).Id.Should().Be(noteShouldBeFound.Id);
                notes.Should().NotContain(n => n.Id.Equals(noteShouldNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenStartAndEndDateFilter_ShouldReturnNotes()
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

                var note1ShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                note1ShouldBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note1ShouldBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note1ShouldBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(note1ShouldBeFound);

                var note2ShouldBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                note2ShouldBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note2ShouldBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note2ShouldBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                context.Notes.Add(note2ShouldBeFound);

                var noteShouldNotBeFound = NoteCommon.CreateNote(database, organisation, null, aatf);
                context.Notes.Add(noteShouldNotBeFound);

                await database.WeeeContext.SaveChangesAsync();

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    StartDateSubmitted = DateTime.Now.AddDays(-1),
                    EndDateSubmitted = DateTime.Now.AddDays(1),
                    AllowedStatuses = new List<NoteStatus>() { NoteStatus.Rejected },
                    AatfId = aatf.Id
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(2);
                notes.Count(n => n.Id == note2ShouldBeFound.Id).Should().Be(1);
                notes.Count(n => n.Id == note1ShouldBeFound.Id).Should().Be(1);
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    SearchRef = $"Z{noteShouldNotBeFound.Reference}",
                    AllowedStatuses = new List<NoteStatus>() { noteShouldNotBeFound.Status },
                };

                var notes = await dataAccess.GetAllNotes(filter);

                notes.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task GetAllNotes_GivenSearchRefWithNoteTypeAlongWithOrganisationAndAatfShouldReturnSingleNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var noteShouldBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote);
                var noteShouldNotBeFound = await SetupSingleNote(context, database);

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                    SearchRef = $"{NoteType.EvidenceNote.DisplayName}{noteShouldBeFound.Reference}",
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

                var filter = new NoteFilter(DateTime.Now.Year)
                {
                    NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
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
