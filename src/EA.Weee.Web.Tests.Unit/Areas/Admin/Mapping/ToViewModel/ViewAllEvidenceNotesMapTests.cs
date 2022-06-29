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
    using Xunit;

    public class ViewAllEvidenceNotesMapTests
    {
        private readonly Fixture fixture;
        private readonly ViewAllEvidenceNotesMap map;
        private readonly IMapper mapper;

        public ViewAllEvidenceNotesMapTests()
        {
            fixture = new Fixture();
            mapper = A.Fake<IMapper>();

            map = new ViewAllEvidenceNotesMap(mapper);
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
        public void Map_GivenViewAllEvidenceNotesMapModelWithNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var source = fixture.Build<ViewAllEvidenceNotesMapModel>()
                .With(s => s.Notes, notes)
                .Create();

            //act
            map.Map(source);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenViewAllEvidenceNotesMapModelWithEmptyNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();
            var source = fixture.Build<ViewAllEvidenceNotesMapModel>()
                .With(s => s.Notes, notes)
                .Create();

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenManageEvidenceNoteViewModel_PropertiesShouldBeMapped()
        {
            //arrange
            var managedEvidenceNoteViewModel = fixture.Create<ManageEvidenceNoteViewModel>();
            var source = fixture.Build<ViewAllEvidenceNotesMapModel>()
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
            var source = fixture.Build<ViewAllEvidenceNotesMapModel>()
                .With(s => s.ManageEvidenceNoteViewModel, managedEvidenceNoteViewModel)
                .Create();

            //act
            var result = map.Map(source);

            // assert 
            result.ManageEvidenceNoteViewModel.Should().BeNull();
        }
    }
}
