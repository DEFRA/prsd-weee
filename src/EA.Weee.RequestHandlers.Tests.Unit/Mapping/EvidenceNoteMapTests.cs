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
    using EA.Weee.Core.Tests.Unit.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
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
            var complianceYear = fixture.Create<short>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.StartDate).Returns(startDate);
            A.CallTo(() => note.EndDate).Returns(endDate);
            A.CallTo(() => note.Recipient.Id).Returns(recipientId);
            A.CallTo(() => note.ComplianceYear).Returns(complianceYear);

            //act
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

            //assert
            result.RejectedDate.Should().Be(date);
        }

        [Fact]
        public void Map_GivenNoteWithReturnedHistory_ReasonShouldBeSet()
        {
            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = fixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Returned);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(note);

            //assert
            result.ReturnedReason.Should().Be(reason);
        }

        [Fact]
        public void Map_GivenNoteWithRejectedHistory_ReasonShouldBeSet()
        {
            //arrange
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var reason = fixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(NoteStatus.Rejected);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(NoteStatus.Rejected);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(note);

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
            var reason = fixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(status);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(status);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(note);

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
            var reason = fixture.Create<string>();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ToStatus).Returns(status);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(status);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

            //assert
            result.RejectedDate.Should().Be(latestDate);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleReturnedHistory_ReasonShouldBeSet()
        {
            //arrange
            var reasonEarly = fixture.Create<string>();
            var returnedDateEarly = DateTime.Now;
            var reasonLate = fixture.Create<string>();
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
            var result = map.Map(note);

            //assert
            result.ReturnedReason.Should().Be(reasonLate);
        }

        [Fact]
        public void Map_GivenNoteWithMultipleRejectedHistory_ReasonShouldBeSet()
        {
            //arrange
            var reasonEarly = fixture.Create<string>();
            var rejectedDateEarly = DateTime.Now;
            var reasonLate = fixture.Create<string>();
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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

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
            var result = map.Map(note);

            //assert
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

            //assert
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

            //assert
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

            //assert
            result.OrganisationData.Should().Be(organisationData);
        }
        
        [Fact]
        public void Map_GivenNote_SchemeOrganisationDataShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            var organisationData = fixture.Create<OrganisationData>();

            A.CallTo(() => note.Recipient.Organisation).Returns(organisation);
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).Returns(organisationData);

            //act
            var result = map.Map(note);

            //assert
            result.RecipientOrganisationData.Should().Be(organisationData);
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

            //assert
            result.EvidenceTonnageData.Count.Should().Be(tonnages.Count);
            result.EvidenceTonnageData.Count.Should().BeGreaterThan(0);
            result.EvidenceTonnageData.Should().BeEquivalentTo(
                tonnages.Select(t =>
                    new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused, null, null)).ToList());
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
                new NoteTransferTonnage(fixture.Create<Guid>(), transferReceive, transferReused) {TransferNote = rejectTransferNote}, // Should not be included
                new NoteTransferTonnage(fixture.Create<Guid>(), transferReceive, transferReused) {TransferNote = approvedTransferNote} // Sums should only be counted from this
            };

            var tonnages = new List<NoteTonnage>()
            {
                new NoteTonnage(Domain.Lookup.WeeeCategory.ConsumerEquipment, totalReceive, totalReuse) { NoteTransferTonnage = transferTonnages },
            };

            A.CallTo(() => rejectTransferNote.Status).Returns(NoteStatus.Rejected);
            A.CallTo(() => approvedTransferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => note.NoteTonnage).Returns(tonnages);

            //Act
            var result = map.Map(note);

            //Assert
            result.EvidenceTonnageData[0].AvailableReceived.Should().Be(expectedAvailableReceive);
            result.EvidenceTonnageData[0].AvailableReused.Should().Be(expectedAvailableReuse);
        }
    }
}
