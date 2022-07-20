namespace EA.Weee.RequestHandlers.Tests.DataAccess.Evidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Weee.DataAccess;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class EvidenceDataAccessUnitTests : SimpleUnitTestBase
    {
        private readonly EvidenceDataAccess evidenceDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly DbContextHelper dbContextHelper;
        private readonly WeeeContext context;
        private Guid userId;
        private readonly Organisation organisation;
        private readonly Organisation recipientOrganisation;
        private readonly List<NoteTransferTonnage> tonnages;
        private readonly short complianceYear;

        public EvidenceDataAccessUnitTests()
        {
            context = A.Fake<WeeeContext>();
            var userContext = A.Fake<IUserContext>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userId = Guid.NewGuid();

            dbContextHelper = new DbContextHelper();
            organisation = A.Fake<Organisation>();
            recipientOrganisation = A.Fake<Organisation>();
            tonnages = new List<NoteTransferTonnage>()
            {
                TestFixture.Create<NoteTransferTonnage>(),
                TestFixture.Create<NoteTransferTonnage>()
            };

            complianceYear = TestFixture.Create<short>();
            A.CallTo(() => userContext.UserId).Returns(userId);

            evidenceDataAccess = new EvidenceDataAccess(context, userContext, genericDataAccess);
        }

        [Fact]
        public async Task AddTransferNote_GivenDraftTransferNote_NoteShouldBeAddedToContext()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);
            
            //act
            await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation, tonnages, NoteStatus.Draft, complianceYear, userId.ToString(), date);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.Aatf == null &&
                                                                           n.CreatedDate.Equals(date) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol == null &&
                                                                           n.WasteType == WasteType.HouseHold &&
                                                                           n.Recipient.Equals(recipientOrganisation) &&
                                                                           n.StartDate.Equals(date) &&
                                                                           n.EndDate.Equals(date) &&
                                                                           n.CreatedById.Equals(userId.ToString()) &&
                                                                           n.NoteType.Equals(NoteType.TransferNote) &&
                                                                           n.Status.Equals(NoteStatus.Draft) &&
                                                                           n.NoteTransferTonnage.Count.Equals(tonnages.Count) &&
                                                                           n.NoteStatusHistory.Count.Equals(0) &&
                                                                           n.NoteTonnage.Count.Equals(0))))
                .MustHaveHappenedOnceExactly();

            foreach (var requestTonnageValue in tonnages)
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTransferTonnage.FirstOrDefault(c =>
                    c.NoteTonnageId.Equals(requestTonnageValue.NoteTonnageId) &&
                    c.Reused.Equals(requestTonnageValue.Reused) &&
                    c.Received.Equals(requestTonnageValue.Received)) != null))).MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task AddTransferNote_GivenSubmittedTransferNote_NoteShouldBeAddedToContext()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);

            //act
            await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation, tonnages, NoteStatus.Submitted, complianceYear, userId.ToString(), date);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.Aatf == null &&
                                                                           n.CreatedDate.Equals(date) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol == null &&
                                                                           n.WasteType == WasteType.HouseHold &&
                                                                           n.Recipient.Equals(recipientOrganisation) &&
                                                                           n.StartDate.Equals(date) &&
                                                                           n.EndDate.Equals(date) &&
                                                                           n.CreatedById.Equals(userId.ToString()) &&
                                                                           n.NoteType.Equals(NoteType.TransferNote) &&
                                                                           n.Status.Equals(NoteStatus.Submitted) &&
                                                                           n.NoteTransferTonnage.Count.Equals(tonnages.Count) &&
                                                                           n.NoteStatusHistory.Count.Equals(1) &&
                                                                           n.NoteTonnage.Count.Equals(0))))
                .MustHaveHappenedOnceExactly();

            foreach (var requestTonnageValue in tonnages)
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTransferTonnage.FirstOrDefault(c =>
                    c.NoteTonnageId.Equals(requestTonnageValue.NoteTonnageId) &&
                    c.Reused.Equals(requestTonnageValue.Reused) &&
                    c.Received.Equals(requestTonnageValue.Received)) != null))).MustHaveHappenedOnceExactly();
            }
            
            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Submitted)]
        [InlineData(Core.AatfEvidence.NoteStatus.Draft)]
        public async Task AddTransferNote_GivenTransferNote_NoteShouldBeAddedAndSaveChangesCalled(Core.AatfEvidence.NoteStatus status)
        {
            //act
            await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation, tonnages, status.ToDomainEnumeration<NoteStatus>(), complianceYear, userId.ToString(), SystemTime.UtcNow);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>._)).MustHaveHappenedOnceExactly().Then(
                A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Submitted)]
        [InlineData(Core.AatfEvidence.NoteStatus.Draft)]
        public async Task AddTransferNote_GivenAddedTransferNote_NoteIdShouldBeReturned(Core.AatfEvidence.NoteStatus status)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => genericDataAccess.Add(A<Note>._)).Returns(note);

            //act
            var result = await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation, tonnages, status.ToDomainEnumeration<NoteStatus>(), complianceYear, userId.ToString(), SystemTime.UtcNow);

            //assert
            result.Should().Be(note);
        }

        [Fact]
        public async Task GetNoteById_GivenNoNoteFound_ArgumentExceptionExpected()
        {
            //arrange
            A.CallTo(() => context.Notes).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Note>()));

            //act
            var exception = await Record.ExceptionAsync(() => evidenceDataAccess.GetNoteById(Guid.NewGuid()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task AddTransferNote_TransferNote_ShouldHave_HouseholdWasteType()
        {
            //act
            await evidenceDataAccess.AddTransferNote(organisation, recipientOrganisation, tonnages, NoteStatus.Draft, complianceYear, userId.ToString(), DateTime.Now);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(x => x.WasteType == WasteType.HouseHold))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateTransferNote_GivenDraftNoteWithDraftNoteStatus_NoteShouldHaveDraftStatus()
        {
            //arrange
            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Draft);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, A.Fake<Organisation>(), new List<NoteTransferTonnage>(),
                NoteStatus.Draft, TestFixture.Create<DateTime>());

            //assert
            updatedNote.Status.Should().Be(NoteStatus.Draft);
        }

        [Fact]
        public async Task UpdateTransferNote_GivenDraftNoteWithSubmittedNoteStatus_NoteShouldHaveSubmittedStatus()
        {
            //arrange
            var note = new Note(A.Fake<Organisation>(), A.Fake<Organisation>(), "user", new List<NoteTransferTonnage>(),
                TestFixture.Create<int>(), WasteType.HouseHold);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, A.Fake<Organisation>(), new List<NoteTransferTonnage>(),
                NoteStatus.Submitted, TestFixture.Create<DateTime>());

            //assert
            updatedNote.Status.Should().Be(NoteStatus.Submitted);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task UpdateTransferNote_GivenNoteWithExistingTonnages_NoteShouldHaveNoteTonnagesUpdatedAddedOrRemoved(NoteStatus status)
        {
            //arrange
            var noteTonnageId1 = TestFixture.Create<Guid>();
            var noteTonnageId2 = TestFixture.Create<Guid>();
            var newNoteTonnage1 = TestFixture.Create<Guid>();
            var newNoteTonnage2 = TestFixture.Create<Guid>();

            var existingTonnages = new List<NoteTransferTonnage>()
            {
                new NoteTransferTonnage(noteTonnageId1, 10, 5),
                new NoteTransferTonnage(noteTonnageId2, 2, 1),
            };

            var updateTonnages = new List<NoteTransferTonnage>()
            {
                new NoteTransferTonnage(noteTonnageId1, 2, 1),
                new NoteTransferTonnage(newNoteTonnage1, 20, 6),
                new NoteTransferTonnage(newNoteTonnage2, 3, null),
            };

            var note = new Note(A.Fake<Organisation>(), A.Fake<Organisation>(), "user", existingTonnages,
                TestFixture.Create<int>(), WasteType.HouseHold);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, A.Fake<Organisation>(), updateTonnages,
                status, TestFixture.Create<DateTime>());

            //assert
            updatedNote.NoteTransferTonnage.Count.Should().Be(4); 
            
            //the note transfer tonnage will still contain the one that should have been removed (noteTonnageId2) but it should have been in the remove call
            A.CallTo(() => genericDataAccess.RemoveMany(A<IEnumerable<NoteTransferTonnage>>
                .That.Contains(existingTonnages.First(nt => nt.NoteTonnageId == noteTonnageId2)))).MustHaveHappenedOnceExactly();

            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnageId1).Received.Should().Be(2);
            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnageId1).Reused.Should().Be(1);
            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == newNoteTonnage1).Received.Should().Be(20);
            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == newNoteTonnage1).Reused.Should().Be(6);
            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == newNoteTonnage2).Received.Should().Be(3);
            updatedNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == newNoteTonnage2).Reused.Should().Be(null);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task UpdateTransferNote_GivenNoteWithChangedRecipient_RecipientShouldBeUpdated(NoteStatus status)
        {
            //arrange
            var originalRecipient = A.Fake<Organisation>();
            var newRecipient = A.Fake<Organisation>();
            var note = new Note(A.Fake<Organisation>(), originalRecipient, "user", new List<NoteTransferTonnage>(),
                TestFixture.Create<int>(), WasteType.HouseHold);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, newRecipient, new List<NoteTransferTonnage>(),
                status, TestFixture.Create<DateTime>());

            //assert
            updatedNote.Recipient.Should().Be(newRecipient);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task UpdateTransferNote_GivenNoteWithNoInChangedRecipient_RecipientShouldNotBeUpdated(NoteStatus status)
        {
            //arrange
            var originalRecipient = A.Fake<Organisation>();
            var note = new Note(A.Fake<Organisation>(), originalRecipient, "user", new List<NoteTransferTonnage>(),
                TestFixture.Create<int>(), WasteType.HouseHold);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, originalRecipient, new List<NoteTransferTonnage>(),
                status, TestFixture.Create<DateTime>());

            //assert
            updatedNote.Recipient.Should().Be(originalRecipient);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task UpdateTransferNote_GivenNote_SaveChangesShouldBeCalled(NoteStatus status)
        {
            //arrange
            var note = new Note(A.Fake<Organisation>(), A.Fake<Organisation>(), "user", new List<NoteTransferTonnage>(),
                TestFixture.Create<int>(), WasteType.HouseHold);

            //act
            var updatedNote = await evidenceDataAccess.UpdateTransfer(note, A.Fake<Organisation>(), new List<NoteTransferTonnage>(),
                status, TestFixture.Create<DateTime>());

            //assert
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
