namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class TransferredOutEvidenceViewModelMapTests 
    {
        private readonly TransferredOutEvidenceViewModelMap map;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public TransferredOutEvidenceViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new TransferredOutEvidenceViewModelMap(mapper);

            fixture = new Fixture();
        }

        [Fact]
        public void TransferredOutEvidenceViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(TransferredOutEvidenceViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<TransferredOutEvidenceNotesSchemeViewModel>>();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.NewGuid(),
                null,
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.Empty,
                fixture.CreateMany<EvidenceNoteData>().ToList(),
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenSchemeNameIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferredOutEvidenceNotesViewModelMapTransfer(Guid.NewGuid(),
                fixture.CreateMany<EvidenceNoteData>().ToList(),
                null,
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>()));

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

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                notes,
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

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

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                notes,
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

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

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                notes,
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

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

            var model = new TransferredOutEvidenceNotesSchemeViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();

            var transfer = new TransferredOutEvidenceNotesViewModelMapTransfer(organisationId,
                notes,
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                fixture.Create<ManageEvidenceNoteViewModel>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }
    }
}
