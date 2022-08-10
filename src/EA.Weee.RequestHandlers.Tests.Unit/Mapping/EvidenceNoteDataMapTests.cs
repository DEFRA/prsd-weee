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
    using Weee.Tests.Core;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteDataMapTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly EvidenceNoteDataMap map;

        public EvidenceNoteDataMapTests()
        {
            mapper = A.Fake<IMapper>();

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
            var source = new ListOfNotesMap(notes, TestFixture.Create<bool>());

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
            var source = new ListOfNotesMap(notes, TestFixture.Create<bool>());
            var scheme = A.Fake<Scheme>();
            var evidenceNoteData = A.Fake<Note>();

            // act
            var result = map.Map(source);

            // assert
            result.Should().BeOfType<ListOfEvidenceNoteDataMap>();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustNotHaveHappened();
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_SourceIsNotEmpty_ResultDataListPropertiesShouldBeMapped(bool includeTonnages)
        {
            // arrange
            var listOfNotes = TestFixture.CreateMany<Note>(3).ToList();
            var categories = TestFixture.CreateMany<int>().ToList();

            var source = new ListOfNotesMap(listOfNotes, includeTonnages)
            {
                CategoryFilter = categories
            };
            
            var evidenceData1 = TestFixture.Create<EvidenceNoteData>();
            var evidenceData2 = TestFixture.Create<EvidenceNoteData>();
            var evidenceData3 = TestFixture.Create<EvidenceNoteData>();

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(a => a.IncludeTonnage == includeTonnages && a.IncludeHistory == false && a.CategoryFilter.SequenceEqual(categories) && a.Note.Equals(listOfNotes.ElementAt(0))))).Returns(evidenceData1);
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(a => a.IncludeTonnage == includeTonnages && a.IncludeHistory == false && a.CategoryFilter.SequenceEqual(categories) && a.Note.Equals(listOfNotes.ElementAt(1))))).Returns(evidenceData2);
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMap>.That.Matches(a => a.IncludeTonnage == includeTonnages && a.IncludeHistory == false && a.CategoryFilter.SequenceEqual(categories) && a.Note.Equals(listOfNotes.ElementAt(2))))).Returns(evidenceData3);

            // act
            var result = map.Map(source);

            // assert
            result.ListOfEvidenceNoteData.ElementAt(0).Should().BeEquivalentTo(evidenceData1);
            result.ListOfEvidenceNoteData.ElementAt(1).Should().BeEquivalentTo(evidenceData2);
            result.ListOfEvidenceNoteData.ElementAt(2).Should().BeEquivalentTo(evidenceData3);
        }
    }
}