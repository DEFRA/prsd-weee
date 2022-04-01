namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Domain.Scheme;
    using Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Xunit;

    public class NoteTests
    {
        private readonly Organisation organisation;
        private readonly Scheme scheme;
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly WasteType wasteType;
        private readonly Protocol protocol;
        private readonly Aatf aatf;
        private readonly string createdBy;
        private readonly IEnumerable<NoteTonnage> tonnages;
        private NoteStatus status;
        private NoteType noteType;

        public NoteTests()
        {
            var fixture = new Fixture();
            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            startDate = DateTime.Now.AddDays(1);
            endDate = DateTime.Now.AddDays(2);
            wasteType = fixture.Create<WasteType>();
            protocol = fixture.Create<Protocol>();
            aatf = A.Fake<Aatf>();
            createdBy = fixture.Create<string>();
            tonnages = fixture.CreateMany<NoteTonnage>();
            status = NoteStatus.Draft;
            noteType = NoteType.EvidenceNote;
        }

        [Fact]
        public void Note_Constructor_GivenNullOrganisationArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(null, A.Fake<Scheme>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                NoteType.EvidenceNote,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenNullSchemeArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), null,
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                NoteType.EvidenceNote,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenDefaultStartDateSchemeArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Scheme>(),
                DateTime.MinValue,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                NoteType.EvidenceNote,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Note_Constructor_GivenDefaultEndDateSchemeArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Scheme>(),
                DateTime.Now,
                DateTime.MinValue,
                null,
                null,
                A.Fake<Aatf>(),
                NoteType.EvidenceNote,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Note_Constructor_GivenNullAatfArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Scheme>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                NoteType.EvidenceNote,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenNullTonnagesArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Scheme>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                NoteType.EvidenceNote,
                "created",
                null));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenDraftEvidenceNoteValues_PropertiesShouldBeSet()
        {
            var date = DateTime.Now;

            SystemTime.Freeze(date);

            noteType = NoteType.EvidenceNote;
            status = NoteStatus.Draft;
            
            var result = CreateNote();

            ShouldBeEqualTo(result, date);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void Note_Constructor_GivenSubmittedEvidenceNoteValues_PropertiesShouldBeSet()
        {
            var date = DateTime.Now;

            SystemTime.Freeze(date);

            noteType = NoteType.EvidenceNote;

            var result = CreateNote();

            ShouldBeEqualTo(result, date);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void Note_Constructor_GivenSubmittedTransferNoteValues_PropertiesShouldBeSet()
        {
            var date = DateTime.Now;

            SystemTime.Freeze(date);

            noteType = NoteType.TransferNote;

            var result = CreateNote();

            ShouldBeEqualTo(result, date);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void Note_Constructor_GivenDraftTransferNoteValues_PropertiesShouldBeSet()
        {
            var date = DateTime.Now;

            SystemTime.Freeze(date);

            noteType = NoteType.TransferNote;
            status = NoteStatus.Draft;

            var result = CreateNote();

            ShouldBeEqualTo(result, date);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void UpdateStatus_GivenDraftToSubmittedStatusUpdate_StatusShouldBeUpdated()
        {
            //arrange
            var note = CreateNote();
            
            //act
            note.UpdateStatus(NoteStatus.Submitted, "user");

            //asset
            note.Status.Should().Be(NoteStatus.Submitted);
        }

        [Fact]
        public void UpdateStatus_GivenDraftToSubmittedStatusUpdate_ShouldAddStatusHistory()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            SystemTime.Freeze(date);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user");

            //asset
            note.NoteStatusHistory.Count.Should().Be(1);
            note.NoteStatusHistory.ElementAt(0).ChangedById.Should().Be("user");
            note.NoteStatusHistory.ElementAt(0).FromStatus.Should().Be(NoteStatus.Draft);
            note.NoteStatusHistory.ElementAt(0).ToStatus.Should().Be(NoteStatus.Submitted);
            note.NoteStatusHistory.ElementAt(0).ChangedDate.Should().Be(date);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void UpdateStatus_GivenDraftStatusToDraftStatusUpdate_InvalidOperationExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var result = Record.Exception(() => note.UpdateStatus(NoteStatus.Draft, "user"));

            //asset
            result.Should().BeOfType<InvalidOperationException>();
        }

        private void ShouldBeEqualTo(Note result, DateTime date)
        {
            result.Organisation.Should().Be(organisation);
            result.Recipient.Should().Be(scheme);
            result.Aatf.Should().Be(aatf);
            result.WasteType.Should().Be(wasteType);
            result.Protocol.Should().Be(protocol);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.NoteTonnage.Should().BeEquivalentTo(tonnages);
            result.CreatedDate.Should().Be(date);
            result.CreatedById.Should().Be(createdBy);
            result.NoteType.Should().Be(noteType);
            result.Status.Should().Be(status);
        }

        public Note CreateNote()
        {
            return new Note(organisation,
                scheme,
                startDate,
                endDate,
                wasteType,
                protocol,
                aatf,
                noteType,
                createdBy,
                tonnages.ToList());
        }
    }
}
