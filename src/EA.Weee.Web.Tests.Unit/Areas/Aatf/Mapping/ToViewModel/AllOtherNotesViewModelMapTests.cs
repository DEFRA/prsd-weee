namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core;
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
    using Xunit;

    public class AllOtherNotesViewModelMapTests
    {
        private readonly AllOtherNotesViewModelMap map;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public AllOtherNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new AllOtherNotesViewModelMap(mapper);

            fixture = new Fixture();
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
            var exception = Record.Exception(() => map.Map(null));

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
            var aatfId = Guid.NewGuid();
 
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);

            //act
            map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);

            //act
            map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);

            //act
            var result = map.Map(transfer);

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

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = map.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenApprovedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].ApprovedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Approved;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = map.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].SubmittedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Submitted;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = map.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenRejectedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].RejectedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Rejected;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, SystemTime.Now);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = map.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }
    }
}
