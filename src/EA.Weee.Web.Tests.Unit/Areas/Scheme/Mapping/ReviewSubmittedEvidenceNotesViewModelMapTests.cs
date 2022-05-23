namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelMapTests
    {
        public readonly ReviewSubmittedEvidenceNotesViewModelMap Map;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public ReviewSubmittedEvidenceNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            Map = new ReviewSubmittedEvidenceNotesViewModelMap(mapper);

            fixture = new Fixture();
        }

        [Fact]
        public void ReviewSubmittedEvidenceNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(ReviewSubmittedEvidenceNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<ReviewSubmittedManageEvidenceNotesSchemeViewModel>>();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.NewGuid(), null, "Test", fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.Empty, fixture.CreateMany<EvidenceNoteData>().ToList(), "Test", fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenSchemeNameIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new ReviewSubmittedEvidenceNotesViewModelMapTransfer(Guid.NewGuid(), fixture.CreateMany<EvidenceNoteData>().ToList(), null, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>()
            };

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            Map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            Map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            //act
            var result = Map.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>(),
                 fixture.Create<EvidenceNoteRowViewModel>(),
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var organisationId = Guid.NewGuid();

            var transfer = new ReviewSubmittedEvidenceNotesViewModelMapTransfer(organisationId, 
                notes, 
                "Test", 
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);
            
            //act
            var result = Map.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }
    }
}
