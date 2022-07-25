namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewAllEvidenceNotesMapTests : SimpleUnitTestBase
    {
        private readonly ViewAllEvidenceNotesMap map;
        private readonly IMapper mapper;

        public ViewAllEvidenceNotesMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new ViewAllEvidenceNotesMap(mapper);
        }

        [Fact]
        public void ShouldBeDerivedFromViewAllEvidenceNotesMapBase()
        {
            typeof(ViewAllEvidenceNotesMap).Should().BeDerivedFrom<ViewAllNotesMapBase<ViewAllEvidenceNotesViewModel>>();
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
            var source = new ViewAllEvidenceNotesMapTransfer(noteData, null);

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

            var source = new ViewAllEvidenceNotesMapTransfer(noteData, null);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenManageEvidenceNoteViewModel_PropertiesShouldBeMapped()
        {
            //arrange
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var source = TestFixture.Build<ViewAllEvidenceNotesMapTransfer>()
                .With(s => s.ManageEvidenceNoteViewModel, managedEvidenceNoteViewModel)
                .Create();

            //act
            var result = map.Map(source);

            // assert 
            result.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(managedEvidenceNoteViewModel);
        }

        [Fact]
        public void Map_GivenNullManageEvidenceNoteViewModel_ManageEvidenceNoteViewModelResultShouldBeNull()
        {
            //arrange
            var managedEvidenceNoteViewModel = (ManageEvidenceNoteViewModel)null;
            var source = TestFixture.Build<ViewAllEvidenceNotesMapTransfer>()
                .With(s => s.ManageEvidenceNoteViewModel, managedEvidenceNoteViewModel)
                .Create();

            //act
            var result = map.Map(source);

            // assert 
            result.ManageEvidenceNoteViewModel.Should().BeNull();
        }
    }
}
