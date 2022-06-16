namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using Xunit;
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
        public void Map_GivenNullTransferScheme_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(new TransferNoteMapTransfer(null, A.Dummy<Note>())));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullTransferNote_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), null)));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenTransferNote_StandardPropertiesShouldBeMapped()
        {
            //arrange
            var id = fixture.Create<Guid>();
            var reference = fixture.Create<int>();
            var complianceYear = fixture.Create<short>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Id).Returns(id);
            A.CallTo(() => note.Reference).Returns(reference);
            A.CallTo(() => note.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => note.ComplianceYear).Returns(complianceYear);

            //act
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //arrange
            result.Id.Should().Be(id);
            result.Reference.Should().Be(reference);
            result.Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
            result.SubmittedDate.Should().BeNull();
            result.ApprovedDate.Should().BeNull();
            result.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public void Map_GivenTransferNote_TransferredOrganisationShouldBeMapped()
        {
            //arrange
            var organisation = A.Fake<Organisation>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Organisation).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNote_RecipientOrganisationShouldBeMapped()
        {
            //arrange
            var organisation = A.Fake<Organisation>();
            
            var note = A.Fake<Note>();
            A.CallTo(() => note.Recipient.Organisation).Returns(organisation);

            //act
            map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferNote_RecipientShouldBeMapped()
        {
            //arrange
            var scheme = A.Fake<Scheme>();

            var note = A.Fake<Note>();
            A.CallTo(() => note.Recipient).Returns(scheme);

            //act
            map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenTransferScheme_TransferSchemeShouldBeMapped()
        {
            //arrange
            var scheme = A.Fake<Scheme>();

            //act
            map.Map(new TransferNoteMapTransfer(scheme, A.Dummy<Note>()));

            //assert
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenMappedData_MappedDataShouldBeReturned()
        {
            //arrange
            var schemeData = A.Fake<SchemeData>();
            var transferredSchemeData = A.Fake<SchemeData>();
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).ReturnsNextFromSequence(schemeData, transferredSchemeData);
            var transferredOrganisationData = A.Fake<OrganisationData>();
            var recipientOrganisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._)).ReturnsNextFromSequence(transferredOrganisationData, recipientOrganisationData);
            var note = A.Fake<Note>();

            //act
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            result.RecipientOrganisationData.Should().Be(recipientOrganisationData);
            result.TransferredOrganisationData.Should().Be(transferredOrganisationData);
            result.RecipientSchemeData.Should().Be(schemeData);
            result.TransferredSchemeData.Should().Be(transferredSchemeData);
        }

        [Fact]
        public void Map_GivenTransferNoteTonnages_TransferNoteTonnageDataShouldBeReturned()
        {
            //arrange
            var note = A.Fake<Note>();
            
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.CategoryId).Returns(WeeeCategory.AutomaticDispensers);
            A.CallTo(() => noteTonnage1.Received).Returns(1);
            A.CallTo(() => noteTonnage1.Reused).Returns(2);
            
            var noteTonnage2 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage2.CategoryId).Returns(WeeeCategory.CoolingApplicancesContainingRefrigerants);
            A.CallTo(() => noteTonnage2.Received).Returns(3);
            A.CallTo(() => noteTonnage2.Reused).Returns(4);
            
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            var noteTonnage1Id = Guid.NewGuid();
            A.CallTo(() => noteTransferTonnage1.Id).Returns(noteTonnage1Id);
            A.CallTo(() => noteTransferTonnage1.NoteTonnage).Returns(noteTonnage1);
            A.CallTo(() => noteTransferTonnage1.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage1.Reused).Returns(6);

            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            var noteTonnage2Id = Guid.NewGuid();
            A.CallTo(() => noteTransferTonnage2.Id).Returns(noteTonnage2Id);
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
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Id.Should().Be(noteTonnage1Id);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.CategoryId.Should().Be(Core.DataReturns.WeeeCategory.AutomaticDispensers);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Received.Should().Be(1);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.Reused.Should().Be(2);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.TransferredReceived.Should().Be(5);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).EvidenceTonnageData.TransferredReused.Should().Be(6);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Id.Should().Be(noteTonnage2Id);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.CategoryId.Should().Be(Core.DataReturns.WeeeCategory.CoolingApplicancesContainingRefrigerants);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Received.Should().Be(3);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.Reused.Should().Be(4);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.TransferredReceived.Should().Be(7);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).EvidenceTonnageData.TransferredReused.Should().Be(8);
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
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

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
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

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
            A.CallTo(() => noteTonnageNote1.NoteType).Returns(NoteType.TransferNote);
            A.CallTo(() => noteTonnageNote1.Reference).Returns(1);
            A.CallTo(() => noteTonnage1.Note).Returns(noteTonnageNote1);

            var noteTonnage2 = A.Fake<NoteTonnage>();
            var noteTonnageNote2 = A.Fake<Note>();
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
            var result = map.Map(new TransferNoteMapTransfer(A.Dummy<Scheme>(), note));

            //assert
            result.TransferEvidenceNoteTonnageData.ElementAt(0).Reference.Should().Be(1);
            result.TransferEvidenceNoteTonnageData.ElementAt(0).Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).Reference.Should().Be(2);
            result.TransferEvidenceNoteTonnageData.ElementAt(1).Type.Should().Be(Core.AatfEvidence.NoteType.Transfer);
        }
    }
}
