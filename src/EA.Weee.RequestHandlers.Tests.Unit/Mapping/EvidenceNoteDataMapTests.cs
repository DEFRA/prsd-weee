namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Castle.Core.Internal;
    using Core.AatfEvidence;
    using Core.Scheme;
    using Domain.Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteDataMapTests
    {
        private readonly IMapper mapper;
        private readonly EvidenceNoteDataMap map;
        private readonly Fixture fixture;

        public EvidenceNoteDataMapTests()
        {
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            map = new EvidenceNoteDataMap(mapper);
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            // act
            Action action = () => map.Map(null);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_SourceIsEmpty_ShouldReturnResultBeOfTypeListOfEvidenceNoteDataMap()
        {
            // arrange
            var notes = new List<Note>();
            var source = new ListOfNotesMap(notes);

            // act
            var result = map.Map(source);

            // assert
            result.Should().BeOfType<ListOfEvidenceNoteDataMap>();
        }

        [Fact]
        public void Map_SourceIsEmpty_ShouldNotCallAnyMappers()
        {
            // arrange
            var notes = new List<Note>();
            var source = new ListOfNotesMap(notes);
            var scheme = A.Fake<Scheme>();
            var evidenceNoteData = A.Fake<Note>();

            // act
            var result = map.Map(source);

            // assert
            result.Should().BeOfType<ListOfEvidenceNoteDataMap>();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustNotHaveHappened();
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(e => e.Note.Equals(evidenceNoteData) && e.CategoryFilter.IsNullOrEmpty()))).MustNotHaveHappened();
        }

        [Fact]
        public void Map_SourceIsNotEmpty_ResultDataListPropertiesShouldBeMapped()
        {
            // arrange
            var listOfNotes = fixture.CreateMany<Note>().ToList();
            var source = new ListOfNotesMap(listOfNotes);
            var schemeData = fixture.Create<SchemeData>();
     
            var evidenceData1 = fixture.Create<EvidenceNoteData>();
            var evidenceData2 = fixture.Create<EvidenceNoteData>();
            var evidenceData3 = fixture.Create<EvidenceNoteData>();

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).ReturnsNextFromSequence(evidenceData3);
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).ReturnsNextFromSequence(evidenceData2);
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).ReturnsNextFromSequence(evidenceData1);

            // act
            var result = map.Map(source);

            // assert
            result.ListOfEvidenceNoteData.ElementAt(0).Should().BeEquivalentTo(evidenceData1);
            result.ListOfEvidenceNoteData.ElementAt(1).Should().BeEquivalentTo(evidenceData2);
            result.ListOfEvidenceNoteData.ElementAt(2).Should().BeEquivalentTo(evidenceData3);
        }
    }
}