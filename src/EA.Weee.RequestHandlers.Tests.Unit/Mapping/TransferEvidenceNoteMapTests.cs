namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using EA.Prsd.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteType = Domain.Evidence.NoteType;
    using Scheme = Domain.Scheme.Scheme;

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
        public void TransferEvidenceNoteMap_ShouldBeDerivedFromEvidenceNoteDataMapBase()
        {
            typeof(TransferEvidenceNoteMap).Should().BeDerivedFrom<EvidenceNoteDataMapBase<TransferEvidenceNoteData>>();
        }

        [Fact]
        public void Map_GivenNullTransferNote_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(new TransferNoteMapTransfer(null)));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenTransferNote_StandardPropertiesShouldBeMapped()
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.SubmittedDate.Should().BeNull();
            result.ApprovedDate.Should().BeNull();
            result.ReturnedDate.Should().BeNull();
            result.ComplianceYear.Should().Be(complianceYear);
            result.RejectedDate.Should().BeNull();
        }

        [Fact]
        public void Map_GivenTransferNote_TransferredOrganisationShouldBeMapped()
        {
            //arrange
            var organisation = A.Fake<Organisation>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNote_RecipientOrganisationShouldBeMapped()
        {
            //arrange
            var organisation = A.Fake<Organisation>();
            
            var note = A.Fake<Note>();
            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNote_RecipientShouldBeMapped()
        {
            //arrange
            var organisation = A.Fake<Organisation>();
            var scheme = A.Fake<Scheme>();
            var note = A.Fake<Note>();

            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => note.Recipient).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNoteOrganisationIsBalancingOrganisation_TransferSchemeShouldNotBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();
            var scheme = A.Fake<Scheme>();
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(note.Organisation.Scheme)).MustNotHaveHappened();
        }

        [Fact]
        public void Map_GivenTransferNoteOrganisationIsNotBalancingOrganisation_TransferSchemeShouldBeMapped()
        {
            //arrange
            var scheme = A.Fake<Scheme>();
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => note.Recipient).Returns(organisation);
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { scheme });

            //act
            map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenMappedDataWhereOrganisationIsNotBalancingScheme_MappedDataShouldBeReturned()
        {
            //arrange
            var schemeData = A.Fake<SchemeData>();
            var transferredSchemeData = A.Fake<SchemeData>();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).ReturnsNextFromSequence(schemeData, transferredSchemeData);
            var transferredOrganisationData = A.Fake<OrganisationData>();
            var recipientOrganisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._)).ReturnsNextFromSequence(transferredOrganisationData, recipientOrganisationData);
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.RecipientOrganisationData.Should().Be(recipientOrganisationData);
            result.TransferredOrganisationData.Should().Be(transferredOrganisationData);
            result.RecipientSchemeData.Should().Be(schemeData);
            result.TransferredSchemeData.Should().Be(transferredSchemeData);
        }

        [Fact]
        public void Map_GivenMappedDataIsBalancingScheme_MappedDataShouldBeReturned()
        {
            //arrange
            var schemeData = A.Fake<SchemeData>();
            var transferredSchemeData = A.Fake<SchemeData>();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).ReturnsNextFromSequence(schemeData, transferredSchemeData);
            var transferredOrganisationData = A.Fake<OrganisationData>();
            var recipientOrganisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._)).ReturnsNextFromSequence(transferredOrganisationData, recipientOrganisationData);
            var note = A.Fake<Note>();
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(A.Fake<ProducerBalancingScheme>());
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.RecipientOrganisationData.Should().Be(recipientOrganisationData);
            result.TransferredOrganisationData.Should().Be(transferredOrganisationData);
            result.RecipientSchemeData.Should().Be(schemeData);
            result.TransferredSchemeData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenTransferNoteTonnages_TransferNoteTonnageDataShouldBeReturned()
        {
            //arrange
            var note = A.Fake<Note>();

            var noteTonnage1 = A.Fake<NoteTonnage>();
            var originatingNoteTonnage1 = Guid.NewGuid();
            A.CallTo(() => noteTonnage1.CategoryId).Returns(WeeeCategory.AutomaticDispensers);
            A.CallTo(() => noteTonnage1.Received).Returns(1);
            A.CallTo(() => noteTonnage1.Reused).Returns(2);
            A.CallTo(() => noteTonnage1.Id).Returns(originatingNoteTonnage1);

            var noteTonnage2 = A.Fake<NoteTonnage>();
            var originatingNoteTonnage2 = Guid.NewGuid();
            A.CallTo(() => noteTonnage2.CategoryId).Returns(WeeeCategory.CoolingApplicancesContainingRefrigerants);
            A.CallTo(() => noteTonnage2.Received).Returns(3);
            A.CallTo(() => noteTonnage2.Reused).Returns(4);
            A.CallTo(() => noteTonnage2.Id).Returns(originatingNoteTonnage2);

            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var noteTransferTonnage1Id = Guid.NewGuid();
            
            A.CallTo(() => noteTransferTonnage1.Id).Returns(noteTransferTonnage1Id);
            A.CallTo(() => noteTransferTonnage1.NoteTonnage).Returns(noteTonnage1);
            A.CallTo(() => noteTransferTonnage1.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage1.Reused).Returns(6);

            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            var noteTransferTonnage2Id = Guid.NewGuid();
            A.CallTo(() => noteTransferTonnage2.Id).Returns(noteTransferTonnage2Id);
            A.CallTo(() => noteTransferTonnage2.Received).Returns(7);
            A.CallTo(() => noteTransferTonnage2.Reused).Returns(8);
            A.CallTo(() => noteTransferTonnage2.NoteTonnage).Returns(noteTonnage2);
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Id.Should().Be(noteTransferTonnage1Id);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.CategoryId.Should().Be(Core.DataReturns.WeeeCategory.AutomaticDispensers);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Received.Should().Be(1);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Reused.Should().Be(2);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.TransferredReceived.Should().Be(5);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.TransferredReused.Should().Be(6);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.OriginatingNoteTonnageId.Should().Be(originatingNoteTonnage1);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Id.Should().Be(noteTransferTonnage2Id);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.CategoryId.Should().Be(Core.DataReturns.WeeeCategory.CoolingApplicancesContainingRefrigerants);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Received.Should().Be(3);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Reused.Should().Be(4);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.TransferredReceived.Should().Be(7);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.TransferredReused.Should().Be(8);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.OriginatingNoteTonnageId.Should().Be(originatingNoteTonnage2);
        }

        [Fact]
        public void Map_GivenTransferNoteTonnages_OriginalAatfShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();

            var noteTonnage1 = A.Fake<NoteTonnage>();
            var noteTonnageNote1 = A.Fake<Note>();
            var aatf1 = A.Fake<Aatf>();
            A.CallTo(() => noteTonnage1.Note).Returns(noteTonnageNote1);
            A.CallTo(() => noteTonnageNote1.Aatf).Returns(aatf1);

            var noteTonnage2 = A.Fake<NoteTonnage>();
            var noteTonnageNote2 = A.Fake<Note>();
            var aatf2 = A.Fake<Aatf>();
            A.CallTo(() => noteTonnage2.Note).Returns(noteTonnageNote2);
            A.CallTo(() => noteTonnageNote2.Aatf).Returns(aatf2);

            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.NoteTonnage).Returns(noteTonnage1);

            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage2.NoteTonnage).Returns(noteTonnage2);

            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            A.CallTo(() => mapper.Map<Aatf, AatfData>(aatf1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Aatf, AatfData>(aatf2)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNoteTonnages_OriginalAatfShouldBeMappedAndReturned()
        {
            //arrange
            var note = A.Fake<Note>();
            var aatf1 = A.Fake<AatfData>();
            var aatf2 = A.Fake<AatfData>();

            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();

            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => mapper.Map<Aatf, AatfData>(A<Aatf>._)).ReturnsNextFromSequence(aatf1, aatf2);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.TransferEvidenceNoteTonnageData.ElementAt(0).OriginalAatf.Should().Be(aatf1);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).OriginalAatf.Should().Be(aatf2);
        }

        [Fact]
        public void Map_GivenTransferNoteTonnages_OriginalNotePropertiesShouldBeMapped()
        {
            //arrange
            var note = A.Fake<Note>();

            var noteTonnage1 = A.Fake<NoteTonnage>();
            var noteTonnageNote1 = A.Fake<Note>();
            var note1Id = fixture.Create<Guid>();
            A.CallTo(() => noteTonnageNote1.Id).Returns(note1Id);
            A.CallTo(() => noteTonnageNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => noteTonnageNote1.Reference).Returns(1);
            A.CallTo(() => noteTonnage1.Note).Returns(noteTonnageNote1);

            var noteTonnage2 = A.Fake<NoteTonnage>();
            var noteTonnageNote2 = A.Fake<Note>();
            var note2Id = fixture.Create<Guid>();
            A.CallTo(() => noteTonnageNote2.Id).Returns(note2Id);
            A.CallTo(() => noteTonnageNote2.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => noteTonnageNote2.Reference).Returns(2);
            A.CallTo(() => noteTonnage2.Note).Returns(noteTonnageNote2);
            
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.NoteTonnage).Returns(noteTonnage1);

            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage2.NoteTonnage).Returns(noteTonnage2);

            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            A.CallTo(() => note.NoteTransferTonnage).Returns(noteTransferTonnages);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.TransferEvidenceNoteTonnageData.ElementAt(0).OriginalReference.Should().Be(1);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).OriginalNoteId.Should().Be(note1Id);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).OriginalReference.Should().Be(2);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).OriginalNoteId.Should().Be(note2Id);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_SubmittedDateShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Submitted))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.SubmittedDate.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ReturnedDateShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Returned))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.ReturnedDate.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_RejectedDateShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Rejected))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.RejectedDate.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ReturnedReasonShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Returned))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.ReturnedReason.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_RejectedReasonShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Rejected))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Submitted);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Returned);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Rejected);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.RejectedReason.Should().Be(reason);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithReturnedHistoryAndNoteIsNotReturned_ReasonShouldBeNull(Domain.Evidence.NoteStatus status)
        {
            if (status.Equals(Domain.Evidence.NoteStatus.Returned))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.ReturnedReason.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithRejectedHistoryAndNoteIsNotRejected_ReasonShouldBeNull(Domain.Evidence.NoteStatus status)
        {
            if (status.Equals(Domain.Evidence.NoteStatus.Rejected))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Submitted);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Submitted);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.Reason).Returns(reasonLate);
            A.CallTo(() => history2.ChangedDate).Returns(returnedDateLate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Returned);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Returned);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.Reason).Returns(reasonLate);
            A.CallTo(() => history2.ChangedDate).Returns(rejectedDateLate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Rejected);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Rejected);
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.RejectedReason.Should().Be(reasonLate);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNoteWithOtherHistory_ApprovedDateShouldNotBeSet(Domain.Evidence.NoteStatus noteStatus)
        {
            if (noteStatus.Equals(Domain.Evidence.NoteStatus.Approved))
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
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Approved);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            A.CallTo(() => history1.ToStatus).Returns(Domain.Evidence.NoteStatus.Approved);

            var history2 = A.Fake<NoteStatusHistory>();
            A.CallTo(() => history2.ChangedDate).Returns(notLatestDate);
            A.CallTo(() => history2.ToStatus).Returns(Domain.Evidence.NoteStatus.Approved);

            historyList.Add(history1);
            historyList.Add(history2);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

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
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.Type.ToInt().Should().Be(noteType.Value);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public void Map_GivenNote_NoteStatusPropertyShouldBeMapped(Domain.Evidence.NoteStatus noteStatus)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.Status).Returns(noteStatus);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.Status.ToInt().Should().Be(noteStatus.Value);
        }

        [Theory]
        [ClassData(typeof(WasteTypeData))]
        public void Map_GivenNote_WasteTypePropertyShouldBeMapped(Domain.Evidence.WasteType wasteType)
        {
            //arrange
            var note = A.Fake<Note>();

            A.CallTo(() => note.WasteType).Returns(wasteType);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.WasteType.ToInt().Should().Be(wasteType.ToInt());
        }

        [Fact]
        public void Map_GivenNoteWithVoidedHistory_VoidedDateShouldBeSet()
        {
            //arrange
            var date = SystemTime.UtcNow;
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();

            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Void);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.VoidedDate.Should().BeSameDateAs(date);
        }

        [Fact]
        public void Map_GivenNoteWithVoidedHistory_VoidedReasonShouldBeSet()
        {
            //arrange
            var date = SystemTime.UtcNow;
            var reason = "voided reason text";
            var historyList = new List<NoteStatusHistory>();
            var history = A.Fake<NoteStatusHistory>();
            var id = Guid.NewGuid();

            A.CallTo(() => history.Reason).Returns(reason);
            A.CallTo(() => history.ChangedDate).Returns(date);
            A.CallTo(() => history.ToStatus).Returns(Domain.Evidence.NoteStatus.Void);

            historyList.Add(history);

            var note = A.Fake<Note>();
            A.CallTo(() => note.NoteStatusHistory).Returns(historyList);
            A.CallTo(() => note.Status).Returns(Domain.Evidence.NoteStatus.Void);
            A.CallTo(() => note.Id).Returns(id);

            //act
            var result = map.Map(new TransferNoteMapTransfer(note));

            //assert
            result.VoidedReason.Should().BeSameAs(reason);
        }
    }
}
