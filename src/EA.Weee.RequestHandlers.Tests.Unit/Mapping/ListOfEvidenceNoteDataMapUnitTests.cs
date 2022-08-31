namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Domain.Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using System;
    using System.Linq;
    using Core.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class ListOfEvidenceNoteDataMapUnitTests : SimpleUnitTestBase
    {
        private readonly ListOfEvidenceNoteDataMap listOfEvidenceNoteDataMap;
        private readonly IMapper mapper;

        public ListOfEvidenceNoteDataMapUnitTests()
        {
            mapper = A.Fake<IMapper>();

            listOfEvidenceNoteDataMap = new ListOfEvidenceNoteDataMap(mapper);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => listOfEvidenceNoteDataMap.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNotes_NotesShouldBeMapped()
        {
            //arrange
            var notes = TestFixture.CreateMany<Note>().ToList();

            //act
            listOfEvidenceNoteDataMap.Map(notes);

            //assert
            foreach (var note in notes)
            {
                A.CallTo(() => mapper.Map<EvidenceNoteRowMapperObject, EvidenceNoteData>(A<EvidenceNoteRowMapperObject>
                    .That.Matches(e => e.Note.Equals(note)))).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Map_GivenMappedNotes_MappedNotesShouldBeReturned()
        {
            //arrange
            var evidenceNoteData = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() => mapper.Map<EvidenceNoteRowMapperObject, EvidenceNoteData>(A<EvidenceNoteRowMapperObject>._)).ReturnsNextFromSequence(evidenceNoteData.ToArray());

            //act
            var result = listOfEvidenceNoteDataMap.Map(TestFixture.CreateMany<Note>().ToList());

            //assert
            result.Should().BeEquivalentTo(evidenceNoteData);
        }
    }
}
