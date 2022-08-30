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
    using Prsd.Core.Mapper;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;
    using Protocol = Domain.Evidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;
    using WasteType = Domain.Evidence.WasteType;

    public class EvidenceNoteMapTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly EvidenceNoteMap map;

        public EvidenceNoteMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new EvidenceNoteMap(mapper);
        }

        [Fact]
        public void EvidenceNoteMap_ShouldBeDerivedFromEvidenceNoteDataMapBase()
        {
            typeof(EvidenceNoteMap).Should().BeDerivedFrom<EvidenceNoteDataMapBase<EvidenceNoteData>>();
        }

        [Fact]
        public void Map_GivenNote_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var reference = TestFixture.Create<int>();
            var startDate = TestFixture.Create<DateTime>();
            var endDate = TestFixture.Create<DateTime>();
            var recipientId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<short>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.StartDate).Returns(startDate);
            A.CallTo(() => note.EndDate).Returns(endDate);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => note.ComplianceYear).Returns(complianceYear);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.RecipientId.Should().Be(recipientId);
            result.SubmittedDate.Should().BeNull();
            result.ApprovedDate.Should().BeNull();
            result.ReturnedDate.Should().BeNull();
            result.ComplianceYear.Should().Be(complianceYear);
            result.RejectedDate.Should().BeNull();
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

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ReturnedDateShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Returned))
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
            result.ReturnedDate.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_RejectedDateShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Rejected))
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
            result.RejectedDate.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ReturnedReasonShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Returned))
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
            result.ReturnedReason.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_RejectedReasonShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Rejected))
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
            result.RejectedReason.Should().BeNull();
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
        public void Map_GivenNoteWithReturnedHistory_ReturnedDateShouldBeSet()
        {
            //arrange
            var date = DateTime.Now;
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Returned);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ReturnedDate.Should().Be(date);
        }

        [Fact]
        public void Map_GivenNoteWithRejectedHistory_RejectedDateShouldBeSet()
        {
            //arrange
            var date = DateTime.Now;
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Rejected);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RejectedDate.Should().Be(date);
        }

        [Fact]
        public void Map_GivenNoteWithReturnedHistory_ReasonShouldBeSet()
        {
            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = TestFixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Returned);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ReturnedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoteWithRejectedHistory_ReasonShouldBeSet()
        {
            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = TestFixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Rejected);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Rejected);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RejectedReason.Should().Be(reason);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithReturnedHistoryAndNoteIsNotReturned_ReasonShouldBeNull(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Returned))
            {
                return;
            }

            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = TestFixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(status);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(status);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ReturnedReason.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithRejectedHistoryAndNoteIsNotRejected_ReasonShouldBeNull(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Rejected))
            {
                return;
            }

            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = TestFixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(status);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(status);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RejectedReason.Should().BeNull();
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

        [Fact]
        public void Map_GivenNoteWithMultipleReturnedHistory_ReturnedDateShouldBeSet()
        {
            //arrange
            var latestDate = DateTime.Now;
            var notLatestDate = latestDate.AddHours(-1);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.ChangedDate).Returns(latestDate);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Returned);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Returned);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ReturnedDate.Should().Be(latestDate);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleRejectedHistory_RejectedDateShouldBeSet()
        {
            //arrange
            var latestDate = DateTime.Now;
            var notLatestDate = latestDate.AddHours(-1);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.ChangedDate).Returns(latestDate);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Rejected);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Rejected);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RejectedDate.Should().Be(latestDate);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleReturnedHistory_ReasonShouldBeSet()
        {
            //arrange
            var reasonEarly = TestFixture.Create<string>();
            var returnedDateEarly = DateTime.Now;
            var reasonLate = TestFixture.Create<string>();
            var returnedDateLate = DateTime.Now.AddMinutes(10);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.Reason).Returns(reasonEarly);
            A.CallTo(() => history1.ChangedDate).Returns(returnedDateEarly);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Returned);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.Reason).Returns(reasonLate);
            A.CallTo(() => history2.ChangedDate).Returns(returnedDateLate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Returned);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ReturnedReason.Should().Be(reasonLate);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleRejectedHistory_ReasonShouldBeSet()
        {
            //arrange
            var reasonEarly = TestFixture.Create<string>();
            var rejectedDateEarly = DateTime.Now;
            var reasonLate = TestFixture.Create<string>();
            var rejectedDateLate = DateTime.Now.AddMinutes(10);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.Reason).Returns(reasonEarly);
            A.CallTo(() => history1.ChangedDate).Returns(rejectedDateEarly);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Rejected);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.Reason).Returns(reasonLate);
            A.CallTo(() => history2.ChangedDate).Returns(rejectedDateLate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Rejected);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Rejected);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RejectedReason.Should().Be(reasonLate);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ApprovedDateShouldNotBeSet(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Approved))
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
            result.ApprovedDate.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNoteWithApprovedHistory_ApprovedDateShouldBeSet()
        {
            //arrange
            var date = DateTime.Now;
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Approved);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ApprovedDate.Should().BeSameDateAs(date);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleApprovedHistory_ApprovedDateShouldBeSet()
        {
            //arrange
            var latestDate = DateTime.Now;
            var notLatestDate = latestDate.AddMilliseconds(-1);
            var historyList = new List<NoteStatusHistory>();
            var history1 = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history1.ChangedDate).Returns(latestDate);
            A.CallTo(() => history1.ToStatus).Returns(NoteStatus.Approved);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(NoteStatus.Approved);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.ApprovedDate.Should().Be(latestDate);
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
        public void Map_GivenNote_SchemeDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var schemeData = TestFixture.Create<SchemeData>();

            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => note.Recipient).Returns(organisation);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientSchemeData.Should().Be(schemeData);
        }

        [Fact]
        public void Map_GivenNote_AatfDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var aatf = A.Fake<Aatf>();
            var aatfData = TestFixture.Create<AatfData>();

            A.CallTo(() => note.Aatf).Returns(aatf);
            A.CallTo(() => mapper.Map<AatfSimpleMapObject, AatfData>(A<AatfSimpleMapObject>
                .That.Matches(a => a.Aatf.Equals(aatf)))).Returns(aatfData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.AatfData.Should().Be(aatfData);
        }

        [Fact]
        public void Map_GivenSourceWithCurrentSystemDateTime_AatfDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var aatf = A.Fake<Aatf>();
            var aatfData = TestFixture.Create<AatfData>();
            var currentSystemDateTime = TestFixture.Create<DateTime>();

            A.CallTo(() => note.Aatf).Returns(aatf);
            A.CallTo(() => mapper.Map<AatfWithSystemDateMapperObject, AatfData>(A<AatfWithSystemDateMapperObject>.That.Matches(a => a.Aatf.Equals(aatf)))).Returns(aatfData);

            var source = EvidenceNoteWithCriteriaMap(note);
            source.SystemDateTime = currentSystemDateTime;

            //act
            var result = map.Map(source);

            //assert
            result.AatfData.Should().Be(aatfData);
        }

        [Fact]
        public void Map_GivenNote_OrganisationDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var organisationData = TestFixture.Create<OrganisationData>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).Returns(organisationData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationData.Should().Be(organisationData);
        }
        
        [Fact]
        public void Map_GivenNote_SchemeOrganisationDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var organisationData = TestFixture.Create<OrganisationData>();

            A.CallTo(() => note.Recipient).Returns(organisation);
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).Returns(organisationData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.RecipientOrganisationData.Should().Be(organisationData);
        }

        [Fact]
        public void Map_GivenNoteToIncludeTonnageData_TonnageDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, 1, 2),
                new NoteTonnage(Domain.Lookup.WeeeCategory.DisplayEquipment, null, 1.9M),
                new NoteTonnage(Domain.Lookup.WeeeCategory.GasDischargeLampsAndLedLightSources, null, null)
            };

            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeTonnage = true;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceTonnageData.Count.Should().Be(tonnages.Count);
            result.EvidenceTonnageData.Count.Should().BeGreaterThan(0);
            result.EvidenceTonnageData.Should().BeEquivalentTo(
                tonnages.Select(t =>
                    new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused, null, null)).ToList());
        }

        [Fact]
        public void Map_GivenNoteToNotIncludeTonnageData_TonnageDataShouldBeEmpty()
        {
            //arrange
            var note = A.Fake<Note>();
            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, 1, 2),
                new NoteTonnage(Domain.Lookup.WeeeCategory.DisplayEquipment, null, 1.9M),
                new NoteTonnage(Domain.Lookup.WeeeCategory.GasDischargeLampsAndLedLightSources, null, null)
            };

            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeTonnage = false;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceTonnageData.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenNoteWithCategoryFilter_TonnageDataShouldBeMappedWithOnlyCategoriesRequestedReturned()
        {
            //arrange
            var note = A.Fake<Note>();
            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, 1, 2),
                new NoteTonnage(Domain.Lookup.WeeeCategory.DisplayEquipment, 10, 1.9M),
                new NoteTonnage(Domain.Lookup.WeeeCategory.MedicalDevices, 2, 1),
            };

            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            //act
            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.CategoryFilter = new List<int>()
            {
                Domain.Lookup.WeeeCategory.ConsumerEquipment.ToInt(), Domain.Lookup.WeeeCategory.MedicalDevices.ToInt(),
            };
            mapObject.IncludeTonnage = true;

            var result = map.Map(mapObject);

            //assert
            result.EvidenceTonnageData.Count.Should().Be(2);
            result.EvidenceTonnageData.Count.Should().BeGreaterThan(0);
            result.EvidenceTonnageData.ElementAt(0).CategoryId.ToInt().Should()
                .Be(Domain.Lookup.WeeeCategory.ConsumerEquipment.ToInt());
            result.EvidenceTonnageData.ElementAt(0).Received.Value.Should().Be(1);
            result.EvidenceTonnageData.ElementAt(0).Reused.Value.Should().Be(2);
            result.EvidenceTonnageData.ElementAt(1).CategoryId.ToInt().Should()
                .Be(Domain.Lookup.WeeeCategory.MedicalDevices.ToInt());
            result.EvidenceTonnageData.ElementAt(1).Received.Value.Should().Be(2);
            result.EvidenceTonnageData.ElementAt(1).Reused.Value.Should().Be(1);
            result.EvidenceTonnageData.Should().NotContain(a =>
                a.CategoryId.ToInt() == Domain.Lookup.WeeeCategory.DisplayEquipment.ToInt());
        }

        [Theory]
        [InlineData("7.00", "6.00", "6.00", "5.00", "1.00", "1.00")]
        [InlineData("5.00", "5.00", "5.00", "5.00", "0.00", "0.00")]
        [InlineData("5.00", "5.00", "0.00", "0.00", "5.00", "5.00")]
        [InlineData("5.00", "5.00", "6.00", "6.00", "0.00", "0.00")]
        public void Map_GivenNoteTransferTonnage_ShouldSumCorrectly(decimal totalReceive, decimal totalReuse, decimal transferReceive, decimal transferReused, decimal expectedAvailableReceive, decimal expectedAvailableReuse)
        {
            //Arrange
            var note = A.Fake<Note>();

            var rejectTransferNote = A.Fake<Note>();
            var approvedTransferNote = A.Fake<Note>();

            var transferTonnages = new List<NoteTransferTonnage>()
            {
                new NoteTransferTonnage(TestFixture.Create<Guid>(), transferReceive, transferReused) {TransferNote = rejectTransferNote}, // Should not be included
                new NoteTransferTonnage(TestFixture.Create<Guid>(), transferReceive, transferReused) {TransferNote = approvedTransferNote} // Sums should only be counted from this
            };

            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, totalReceive, totalReuse) { NoteTransferTonnage = transferTonnages },
            };

            A.CallTo(() => rejectTransferNote.Status).Returns(NoteStatus.Rejected);
            A.CallTo(() => approvedTransferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeTonnage = true;

            //Act
            var result = map.Map(mapObject);

            //Assert
            result.EvidenceTonnageData[0].AvailableReceived.Should().Be(expectedAvailableReceive);
            result.EvidenceTonnageData[0].AvailableReused.Should().Be(expectedAvailableReuse);
        }

        [Fact]
        public void Map_GivenNoteWhereNoteOriginatingOrganisationHasScheme_OrganisationSchemaDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var schemeData = TestFixture.Create<SchemeData>();
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationSchemaData.Should().Be(schemeData);
        }

        [Fact]
        public void Map_GivenNoteWhereNoteOriginatingOrganisationHasNoScheme_OrganisationSchemaDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var schemeData = TestFixture.Create<SchemeData>();
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>());
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

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
            var schemeData = TestFixture.Create<SchemeData>();
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(null);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            //act
            var result = map.Map(EvidenceNoteWithCriteriaMap(note));

            //assert
            result.OrganisationSchemaData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenNoteWithRelatedTransferNotesAndIncludeHistoryIsTrue_EvidenceNoteHistoryShouldBeCreated()
        {
            //arrange
            var date = new DateTime(2022, 1, 1);
            var transferNote1 = A.Fake<Note>();
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var historyList = new List<NoteStatusHistory>();
            
            var history = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Submitted);
            historyList.Add(history);

            A.CallTo(() => transferNote1.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote1.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote1.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote1.NoteStatusHistory).Returns(historyList);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(transferNote1);

            var transferNote2 = A.Fake<Note>();
            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();

            A.CallTo(() => transferNote2.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote2.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote2.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote2.Status).Returns(NoteStatus.Draft);
            A.CallTo(() => noteTransferTonnage2.TransferNote).Returns(transferNote2);

            var transferNote3 = A.Fake<Note>();
            var noteTransferTonnage3 = A.Fake<NoteTransferTonnage>();

            A.CallTo(() => transferNote3.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote3.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote3.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote3.Status).Returns(NoteStatus.Void);
            A.CallTo(() => noteTransferTonnage3.TransferNote).Returns(transferNote3);

            // add to check for distinct
            var noteTransferTonnage4 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage4.TransferNote).Returns(transferNote1);

            var note = A.Fake<Note>();
            var noteTransferTonnageList1 = new List<NoteTransferTonnage>() { noteTransferTonnage1, noteTransferTonnage2 };
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnageList1);

            var noteTransferTonnageList2 = new List<NoteTransferTonnage>() { noteTransferTonnage3, noteTransferTonnage4 };
            var noteTonnage2 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage2.NoteTransferTonnage).Returns(noteTransferTonnageList2);
            A.CallTo(() => note.NoteTonnage).Returns(new List<NoteTonnage>() { noteTonnage1, noteTonnage2 });

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeHistory = true;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceNoteHistoryData.Count.Should().Be(3);
            result.EvidenceNoteHistoryData.Should().Contain(e => e.Reference == transferNote1.Reference
                                                                 && e.Id == transferNote1.Id
                                                                 && e.SubmittedDate.Equals(date)
                                                                 && e.Status == transferNote1.Status
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteStatus>()
                                                                 && e.Type == transferNote1.NoteType
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteType>());
            result.EvidenceNoteHistoryData.Should().Contain(e => e.Reference == transferNote2.Reference
                                                                 && e.Id == transferNote2.Id
                                                                 && e.SubmittedDate == null 
                                                                 && e.Status == transferNote2.Status
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteStatus>()
                                                                 && e.Type == transferNote2.NoteType
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteType>());
            result.EvidenceNoteHistoryData.Should().Contain(e => e.Reference == transferNote3.Reference
                                                                 && e.Id == transferNote3.Id
                                                                 && e.SubmittedDate == null
                                                                 && e.Status == transferNote3.Status
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteStatus>()
                                                                 && e.Type == transferNote3.NoteType
                                                                     .ToCoreEnumeration<EA.Weee.Core.AatfEvidence.NoteType>());
            result.EvidenceNoteHistoryData.Should().BeInDescendingOrder(e => e.Reference);
        }

        [Fact]
        public void Map_GivenNoteWithNoTransferNotesAndIncludeHistoryIsTrue_EvidenceNoteHistoryShouldBeEmpty()
        {
            //arrange
            var note = A.Fake<Note>();
            var noteTransferTonnageList1 = new List<NoteTransferTonnage>();
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnageList1);

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeHistory = true;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceNoteHistoryData.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenNoteWithRelatedTransferNotesAndIncludeHistoryIsFalse_EvidenceNoteHistoryShouldBeEmpty()
        {
            //arrange
            var date = new DateTime(2022, 1, 1);
            var transferNote1 = A.Fake<Note>();
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();

            A.CallTo(() => transferNote1.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote1.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote1.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(transferNote1);

            var note = A.Fake<Note>();
            var noteTransferTonnageList1 = new List<NoteTransferTonnage>() { noteTransferTonnage1 };
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnageList1);

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeHistory = false;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceNoteHistoryData.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenNoteWithTransferHistoryWithRecipientScheme_TransferredToPropertyShouldBeMapped()
        {
            //arrange
            var date = new DateTime(2022, 1, 1);
            var transferNote1 = A.Fake<Note>();
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var historyList = new List<NoteStatusHistory>();

            var history = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Submitted);
            historyList.Add(history);

            A.CallTo(() => transferNote1.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote1.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote1.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote1.NoteStatusHistory).Returns(historyList);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(transferNote1);

            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var schemeData = TestFixture.Create<SchemeData>();

            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => transferNote1.Recipient).Returns(organisation);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            var note = A.Fake<Note>();
            var noteTransferTonnageList1 = new List<NoteTransferTonnage>() { noteTransferTonnage1 };
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnageList1);
            A.CallTo(() => note.NoteTonnage).Returns(new List<NoteTonnage>() { noteTonnage1 });

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeHistory = true;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceNoteHistoryData.First().TransferredTo.Should().Be(scheme.SchemeName);
        }

        [Fact]
        public void Map_GivenNoteWithTransferHistoryWithNoRecipientScheme_TransferredToPropertyShouldBeEmptyString()
        {
            //arrange
            var date = new DateTime(2022, 1, 1);
            var transferNote1 = A.Fake<Note>();
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var historyList = new List<NoteStatusHistory>();

            var history = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Submitted);
            historyList.Add(history);

            A.CallTo(() => transferNote1.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => transferNote1.Reference).Returns(TestFixture.Create<int>());
            A.CallTo(() => transferNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => transferNote1.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote1.NoteStatusHistory).Returns(historyList);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(transferNote1);

            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var schemeData = TestFixture.Create<SchemeData>();

            var note = A.Fake<Note>();
            var noteTransferTonnageList1 = new List<NoteTransferTonnage>() { noteTransferTonnage1 };
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnageList1);
            A.CallTo(() => note.NoteTonnage).Returns(new List<NoteTonnage>() { noteTonnage1 });

            var mapObject = EvidenceNoteWithCriteriaMap(note);
            mapObject.IncludeHistory = true;

            //act
            var result = map.Map(mapObject);

            //assert
            result.EvidenceNoteHistoryData.First().TransferredTo.Should().Be(string.Empty);
        }

        private EvidenceNoteWithCriteriaMap EvidenceNoteWithCriteriaMap(Note note)
        {
            return new EvidenceNoteWithCriteriaMap(note);
        }
    }
}
