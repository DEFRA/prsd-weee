namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class AllOtherNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly AllOtherNotesViewModelMap allOtherNotesViewModelMap;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public AllOtherNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            allOtherNotesViewModelMap = new AllOtherNotesViewModelMap(mapper);

            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void AllOtherNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(AllOtherNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<AllOtherManageEvidenceNotesViewModel>>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => allOtherNotesViewModelMap.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 3);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedDataAsPagedList()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 3;

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, pageNumber, pageSize);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenApprovedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].ApprovedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Approved;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.ElementAt(0).DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].SubmittedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Submitted;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.ElementAt(0).DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenRejectedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].RejectedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Rejected;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model, 1, 2);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.ElementAt(0).DisplayViewLink.Should().BeTrue();
        }
    }
}
