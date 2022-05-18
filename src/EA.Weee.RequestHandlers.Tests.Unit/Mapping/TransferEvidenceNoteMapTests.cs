namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using AutoFixture;
    using Domain.Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Xunit;

    public class TransferEvidenceNoteMapTests
    {
        private readonly IMapper mapper;
        private readonly TransferEvidenceNoteMap map;
        private readonly Fixture fixture;

        public TransferEvidenceNoteMapTests()
        {
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            map = new TransferEvidenceNoteMap(mapper);
        }

        [Fact]
        public void Map_GivenNote_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var id = fixture.Create<Guid>();
            var reference = fixture.Create<int>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.NoteType).Returns(NoteType.TransferNote);

            //act
            var result = map.Map(note);

            //arrange
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
            result.SubmittedDate.Should().BeNull();
            result.ApprovedDate.Should().BeNull();
        }
    }
}
