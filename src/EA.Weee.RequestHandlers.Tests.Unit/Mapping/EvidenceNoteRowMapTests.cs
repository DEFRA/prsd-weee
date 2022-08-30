namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using Protocol = Domain.Evidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;
    using WasteType = Domain.Evidence.WasteType;

    public class EvidenceNoteRowMapTests : SimpleUnitTestBase
    {
        private readonly EvidenceNoteRowMap map;

        public EvidenceNoteRowMapTests()
        {
            map = new EvidenceNoteRowMap();
        }

        [Fact]
        public void Map_GivenNote_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var reference = TestFixture.Create<int>();
            var startDate = TestFixture.Create<DateTime>();
            var endDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.StartDate).Returns(startDate);
            A.CallTo(() => note.EndDate).Returns(endDate);
            A.CallTo(() => note.ComplianceYear).Returns(complianceYear);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.SubmittedDate.Should().BeNull();
            result.ApprovedDate.Should().BeNull();
            result.ReturnedDate.Should().BeNull();
            result.RejectedDate.Should().BeNull();
            result.ComplianceYear.Should().Be(complianceYear);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_SubmittedDateShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Submitted))
            {
                return;
            }

            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(DateTime.Now);
            A.CallTo(() => history.ToStatus).Returns(noteStatus);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.SubmittedDate.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNoteWithSubmittedHistory_SubmittedDateShouldBeSet()
        {
            //arrange
            var date = DateTime.Now;
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Submitted);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.SubmittedDate.Should().Be(date);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleSubmittedHistory_SubmittedDateShouldBeSet()
        {
            //arrange
            var latestDate = DateTime.Now;
            var notLatestDate = latestDate.AddMilliseconds(-1);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.ChangedDate).Returns(latestDate);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Submitted);
            
            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Submitted);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.SubmittedDate.Should().Be(latestDate);
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void Map_GivenNote_NoteTypePropertyShouldBeMapped(NoteType noteType)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.NoteType).Returns(noteType);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.Type.ToInt().Should().Be(noteType.Value);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNote_NoteStatusPropertyShouldBeMapped(NoteStatus noteStatus)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.Status).Returns(noteStatus);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.Status.ToInt().Should().Be(noteStatus.Value);
        }

        [Theory]
        [ClassData(typeof(ProtocolData))]
        public void Map_GivenNote_ProtocolPropertyShouldBeMapped(Protocol protocol)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.Protocol).Returns(protocol);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.Protocol.ToInt().Should().Be(protocol.ToInt());
        }

        [Theory]
        [ClassData(typeof(WasteTypeData))]
        public void Map_GivenNote_WasteTypePropertyShouldBeMapped(WasteType wasteType)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.WasteType).Returns(wasteType);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.WasteType.ToInt().Should().Be(wasteType.ToInt());
        }

        [Fact]
        public void Map_GivenNoteWithRecipientScheme_RecipientSchemeDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var schemeName = TestFixture.Create<string>();

            A.CallTo(() => scheme.SchemeName).Returns(schemeName);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientSchemeData.SchemeName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenNoteWithNoRecipientScheme_RecipientSchemeDataShouldNotBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var schemeName = TestFixture.Create<string>();

            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientSchemeData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNoteWithAatf_AatfDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var aatf = A.Fake<Aatf>();
            var aatfName = TestFixture.Create<string>();
            
            A.CallTo(() => aatf.Name).Returns(aatfName);
            A.CallTo(() => note.Aatf).Returns(aatf);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.AatfData.Name.Should().Be(aatfName);
        }

        [Fact]
        public void Map_GivenNoteWithNoAatf_AatfDataShouldNotBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            A.CallTo(() => note.Aatf).Returns(null);
            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.AatfData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNote_OrganisationNameShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisationName = TestFixture.Create<string>();
            var organisation = Organisation.CreateRegisteredCompany(organisationName, "12345678910");
            
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationData.Name.Should().Be(organisationName);
            result.OrganisationData.OrganisationName.Should().Be(organisationName);
        }

        [Fact]
        public void Map_GivenNote_OrganisationTradingNameShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisationName = TestFixture.Create<string>();
            var tradingName = TestFixture.Create<string>();
            var organisation = Organisation.CreateSoleTrader(organisationName, tradingName);

            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationData.Name.Should().Be(organisationName);
            result.OrganisationData.OrganisationName.Should().Be(organisationName);
            result.OrganisationData.TradingName.Should().Be(tradingName);
        }

        [Fact]
        public void Map_GivenNote_RecipientOrganisationNameShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisationName = TestFixture.Create<string>();
            var organisation = Organisation.CreateRegisteredCompany(organisationName, "12345678910");

            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientOrganisationData.Name.Should().Be(organisationName);
            result.RecipientOrganisationData.OrganisationName.Should().Be(organisationName);
        }

        [Fact]
        public void Map_GivenNote_WithRecipientOrganisationTradingNameShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisationName = TestFixture.Create<string>();
            var tradingName = TestFixture.Create<string>();
            var organisation = Organisation.CreateSoleTrader(organisationName, tradingName);

            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientOrganisationData.Name.Should().Be(organisationName);
            result.RecipientOrganisationData.OrganisationName.Should().Be(organisationName);
            result.RecipientOrganisationData.TradingName.Should().Be(tradingName);
        }

        [Fact]
        public void Map_GivenNoteWithRecipientOrganisationThatIsBalancingScheme_BalancingSchemePropertyShouldBeSet()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientOrganisationData.IsBalancingScheme.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNoteWithRecipientOrganisationThatIsNotBalancingScheme_BalancingSchemePropertyShouldBeSet()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientOrganisationData.IsBalancingScheme.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenNoteWhereNoteOriginatingOrganisationHasScheme_OrganisationSchemaDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var schemeName = TestFixture.Create<string>();

            A.CallTo(() => scheme.SchemeName).Returns(schemeName);
            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationSchemaData.SchemeName.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenNoteWhereNoteOriginatingOrganisationHasNoScheme_OrganisationSchemaDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>());

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationSchemaData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNoteWhereNoteOriginatingOrganisationHasNullScheme_OrganisationSchemaDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(null);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationSchemaData.Should().BeNull();
        }

        private EvidenceNoteRowMapperObject EvidenceNoteWithCriteriaMap(Note note)
        {
            return new EvidenceNoteRowMapperObject(note);
        }
    }
}
