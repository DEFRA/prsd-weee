namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using Protocol = Domain.Evidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;
    using WasteType = Domain.Evidence.WasteType;

    public class EvidenceNoteMapTests
    {
        private readonly IMapper mapper;
        private readonly EvidenceNoteMap map;
        private readonly Fixture fixture;

        public EvidenceNoteMapTests()
        {
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            map = new EvidenceNoteMap(mapper);
        }

        [Fact]
        public void Map_GivenNote_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var id = fixture.Create<Guid>();
            var reference = fixture.Create<int>();
            var startDate = fixture.Create<DateTime>();
            var endDate = fixture.Create<DateTime>();
            var recipientId = fixture.Create<Guid>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.StartDate).Returns(startDate);
            A.CallTo(() => note.EndDate).Returns(endDate);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);

            //act
            var result = map.Map(note);

            //arrange
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.RecipientId.Should().Be(recipientId);
        }

        public static IEnumerable<object[]> NoteTypes()
        {
            return Enumeration.GetAll<NoteType>().Select(value => new object[] { value });
        }

        [Theory]
        [MemberData(nameof(NoteTypes))]
        public void Map_GivenNote_NoteTypePropertyShouldBeMapped(NoteType noteType)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.NoteType).Returns(noteType);

            //act
            var result = map.Map(note);

            //arrange
            result.Type.ToInt().Should().Be(noteType.Value);
        }

        public static IEnumerable<object[]> NoteStatuses()
        {
            return Enumeration.GetAll<NoteStatus>().Select(value => new object[] { value });
        }

        [Theory]
        [MemberData(nameof(NoteStatuses))]
        public void Map_GivenNote_NoteStatusPropertyShouldBeMapped(NoteStatus noteStatus)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.Status).Returns(noteStatus);

            //act
            var result = map.Map(note);

            //arrange
            result.Status.ToInt().Should().Be(noteStatus.Value);
        }

        public static IEnumerable<object[]> Protocols()
        {
            foreach (var protocol in typeof(Protocol).GetEnumValues())
            {
                yield return new[] { protocol };
            }

            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(Protocols))]
        public void Map_GivenNote_ProtocolPropertyShouldBeMapped(Protocol protocol)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.Protocol).Returns(protocol);

            //act
            var result = map.Map(note);

            //arrange
            result.Protocol.ToInt().Should().Be(protocol.ToInt());
        }

        public static IEnumerable<object[]> WasteTypes()
        {
            foreach (var wasteType in typeof(WasteType).GetEnumValues())
            {
                yield return new[] { wasteType };
            }
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(WasteTypes))]
        public void Map_GivenNote_WasteTypePropertyShouldBeMapped(WasteType wasteType)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.WasteType).Returns(wasteType);

            //act
            var result = map.Map(note);

            //arrange
            result.WasteType.ToInt().Should().Be(wasteType.ToInt());
        }

        [Fact]
        public void Map_GivenNote_SchemeDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var scheme = A.Fake<Scheme>();
            var schemeData = fixture.Create<SchemeData>();

            A.CallTo(() => note.Recipient).Returns(scheme);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            //act
            var result = map.Map(note);

            //arrange
            result.SchemeData.Should().Be(schemeData);
        }

        [Fact]
        public void Map_GivenNote_AatfDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var aatf = A.Fake<Aatf>();
            var aatfData = fixture.Create<AatfData>();

            A.CallTo(() => note.Aatf).Returns(aatf);
            A.CallTo(() => mapper.Map<Aatf, AatfData>(aatf)).Returns(aatfData);

            //act
            var result = map.Map(note);

            //arrange
            result.AatfData.Should().Be(aatfData);
        }

        [Fact]
        public void Map_GivenNote_OrganisationDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var organisationData = fixture.Create<OrganisationData>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).Returns(organisationData);

            //act
            var result = map.Map(note);

            //arrange
            result.OrganisationData.Should().Be(organisationData);
        }

        [Fact]
        public void Map_GivenNote_TonnageDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, 1, 2),
                new NoteTonnage(Domain.Lookup.WeeeCategory.DisplayEquipment, null, 1.9M)
            };

            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            //act
            var result = map.Map(note);

            //arrange
            result.EvidenceTonnageData.Count.Should().Be(tonnages.Count);
            result.EvidenceTonnageData.Count.Should().BeGreaterThan(0);
            result.EvidenceTonnageData.Should().BeEquivalentTo(
                tonnages.Select(t =>
                    new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused)).ToList());
        }
    }
}
