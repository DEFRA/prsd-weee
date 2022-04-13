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
    using Xunit;

    public class EditDraftReturnNotesViewModelMapTests
    {
        public readonly EditDraftReturnNotesViewModelMap Map;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public EditDraftReturnNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            Map = new EditDraftReturnNotesViewModelMap(mapper);

            fixture = new Fixture();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EditDraftReturnNotesViewModelTransfer(Guid.NewGuid(), Guid.NewGuid(), null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EditDraftReturnNotesViewModelTransfer(Guid.Empty, Guid.NewGuid(), fixture.CreateMany<EvidenceNoteData>().ToList()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenEvidenceNotesfIdGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EditDraftReturnNotesViewModelTransfer(Guid.NewGuid(), Guid.Empty, fixture.CreateMany<EvidenceNoteData>().ToList()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
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
 
            var transfer = new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, notes);

            //act
            Map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EditDraftReturnedNote>(A<EditDraftReturnedNotesModel>._)).MustHaveHappened(notes.Count, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, notes);

            //act
            Map.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EditDraftReturnedNote>(A<EditDraftReturnedNotesModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, notes);

            //act
            var result = Map.Map(transfer);

            // assert 
            result.ListOfNotes.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            var returnedNotes = new List<EditDraftReturnedNote>
            {
                 fixture.Create<EditDraftReturnedNote>(),
                 fixture.Create<EditDraftReturnedNote>(),
                 fixture.Create<EditDraftReturnedNote>()
            };

            var model = new EditDraftReturnedNotesViewModel();
            model.ListOfNotes = returnedNotes;

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var transfer = new EditDraftReturnNotesViewModelTransfer(organisationId, aatfId, notes);

            foreach (var note in model.ListOfNotes)
            {
                A.CallTo(() => mapper.Map<EditDraftReturnedNote>(A<EditDraftReturnedNotesModel>._)).ReturnsNextFromSequence(note);
            }

            //act
            var result = Map.Map(transfer);

            // assert
            result.ListOfNotes.Should().NotBeEmpty();
            result.ListOfNotes.Should().BeEquivalentTo(returnedNotes);
        }
    }
}
