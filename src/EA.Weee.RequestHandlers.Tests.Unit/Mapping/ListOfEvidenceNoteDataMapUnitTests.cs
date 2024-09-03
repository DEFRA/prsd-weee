namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Domain.Evidence;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
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
            var notes = new List<Note>();
            notes.Add(CreateNote());

            //act
            listOfEvidenceNoteDataMap.Map(notes);

            //assert
            foreach (var item in notes)
            {
                A.CallTo(() => mapper.Map<EvidenceNoteRowCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteRowCriteriaMapper>
                    .That.Matches(e => e.Note.Equals(item) && e.IncludeTotal == true))).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Map_GivenMappedNotes_MappedNotesShouldBeReturned()
        {
            //arrange
            var evidenceNoteData = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() => mapper.Map<EvidenceNoteRowCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteRowCriteriaMapper>._)).ReturnsNextFromSequence(evidenceNoteData.ToArray());

            var notes = new List<Note>();
            notes.Add(CreateNote());
            notes.Add(CreateNote());
            notes.Add(CreateNote());

            //act
            var result = listOfEvidenceNoteDataMap.Map(notes);

            //assert
            result.Should().BeEquivalentTo(evidenceNoteData);
        }

        private Note CreateNote()
        {
            var organisation = A.Fake<Organisation>();
            var recipientOrganisation = A.Fake<Organisation>();
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(2);
            var wasteType = Domain.Evidence.WasteType.HouseHold;
            var protocol = Domain.Evidence.Protocol.LdaProtocol;
            var aatf = A.Fake<Aatf>();
            var createdBy = TestFixture.Create<string>();
            var tonnages = TestFixture.CreateMany<NoteTonnage>();

            var note = new Note(organisation, recipientOrganisation, startDate, endDate, wasteType, protocol, aatf, createdBy, tonnages.ToList());

            return note;
        }
    }
}
