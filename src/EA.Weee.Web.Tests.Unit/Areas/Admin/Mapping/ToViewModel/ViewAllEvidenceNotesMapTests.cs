namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewAllEvidenceNotesMapTests : SimpleUnitTestBase
    {
        private readonly ViewAllEvidenceNotesMap map;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public ViewAllEvidenceNotesMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new ViewAllEvidenceNotesMap(mapper);

            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void ShouldBeDerivedFromViewAllEvidenceNotesMapBase()
        {
            typeof(ViewAllEvidenceNotesMap).Should().BeDerivedFrom<ListOfNotesViewModelBase<ViewAllEvidenceNotesViewModel>>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenViewAllEvidenceNotesMapModelWithNoteData_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                null,
                SystemTime.Now, 1, 2, TestFixture.CreateMany<int>());

            //act
            map.Map(source);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e => e.SequenceEqual(noteData.Results)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenViewAllEvidenceNotesMapModelWithEmptyNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, new List<EvidenceNoteData>()).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                null,
                SystemTime.Now, 1, 2, TestFixture.CreateMany<int>());

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_PropertiesShouldBeMapped()
        {
            //arrange
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                managedEvidenceNoteViewModel,
                currentDate, 1, 3, TestFixture.CreateMany<int>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_PropertiesShouldBeMappedAsPagedList()
        {
            //arrange
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var pageNumber = 1;
            var pageSize = 3;

            var source = new ViewEvidenceNotesMapTransfer(noteData,
                managedEvidenceNoteViewModel, currentDate, pageNumber, pageSize, TestFixture.CreateMany<int>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenComplianceYearIsNotClosed_ComplianceYearClosedShouldBeFalse()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year).Create();

            var transfer = new ViewEvidenceNotesMapTransfer(
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                model, currentDate, 1, 2, TestFixture.CreateMany<int>().ToList());

            //act
            var result = map.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
