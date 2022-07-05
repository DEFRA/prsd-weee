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
    using Xunit;

    public class EvidenceDataAccessUnitTests : SimpleUnitTestBase
    {
        private readonly EvidenceDataAccess evidenceDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly DbContextHelper dbContextHelper;
        private readonly WeeeContext context;
        private Guid userId;
        private readonly Organisation organisation;
        private readonly Scheme scheme;
        private readonly List<NoteTransferTonnage> tonnages;
        private readonly List<NoteTransferCategory> categories;
        private readonly short complianceYear;

        public EvidenceDataAccessUnitTests()
        {
            context = A.Fake<WeeeContext>();
            var userContext = A.Fake<IUserContext>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            userId = Guid.NewGuid();

            dbContextHelper = new DbContextHelper();
            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            tonnages = new List<NoteTransferTonnage>()
            {
                TestFixture.Create<NoteTransferTonnage>(),
                TestFixture.Create<NoteTransferTonnage>()
            };
            categories = new List<NoteTransferCategory>()
            {
                TestFixture.Create<NoteTransferCategory>(),
                TestFixture.Create<NoteTransferCategory>()
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
            await evidenceDataAccess.AddTransferNote(organisation, scheme, categories, tonnages, NoteStatus.Draft, complianceYear, userId.ToString(), date);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.Aatf == null &&
                                                                           n.CreatedDate.Equals(date) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol == null &&
                                                                           n.WasteType == WasteType.HouseHold &&
                                                                           n.Recipient.Equals(scheme) &&
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

            foreach (var category in categories) 
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTransferCategories.FirstOrDefault(c =>
                    c.CategoryId.Equals(category.CategoryId)) != null))).MustHaveHappenedOnceExactly();
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
            await evidenceDataAccess.AddTransferNote(organisation, scheme, categories, tonnages, NoteStatus.Submitted, complianceYear, userId.ToString(), date);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.Aatf == null &&
                                                                           n.CreatedDate.Equals(date) &&
                                                                           n.Organisation.Equals(organisation) &&
                                                                           n.Protocol == null &&
                                                                           n.WasteType == WasteType.HouseHold &&
                                                                           n.Recipient.Equals(scheme) &&
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
            
            foreach (var category in categories)
            {
                A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(n => n.NoteTransferCategories.FirstOrDefault(c =>
                    c.CategoryId.Equals(category.CategoryId)) != null))).MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(Core.AatfEvidence.NoteStatus.Submitted)]
        [InlineData(Core.AatfEvidence.NoteStatus.Draft)]
        public async Task AddTransferNote_GivenTransferNote_NoteShouldBeAddedAndSaveChangesCalled(Core.AatfEvidence.NoteStatus status)
        {
            //act
            await evidenceDataAccess.AddTransferNote(organisation, scheme, categories, tonnages, status.ToDomainEnumeration<NoteStatus>(), complianceYear, userId.ToString(), SystemTime.UtcNow);

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
            var id = Guid.NewGuid();
            A.CallTo(() => note.Id).Returns(id);

            A.CallTo(() => genericDataAccess.Add(A<Note>._)).Returns(note);

            //act
            var result = await evidenceDataAccess.AddTransferNote(organisation, scheme, categories, tonnages, status.ToDomainEnumeration<NoteStatus>(), complianceYear, userId.ToString(), SystemTime.UtcNow);

            //assert
            result.Should().Be(id);
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
            await evidenceDataAccess.AddTransferNote(organisation, scheme, categories, tonnages, NoteStatus.Draft, complianceYear, userId.ToString(), DateTime.Now);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<Note>.That.Matches(x => x.WasteType == WasteType.HouseHold))).MustHaveHappenedOnceExactly();
        }
    }
}
