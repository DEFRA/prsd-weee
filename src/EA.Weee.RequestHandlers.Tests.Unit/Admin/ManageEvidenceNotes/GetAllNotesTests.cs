namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetAllNotesTests
    {
        private readonly Fixture fixture;

        public GetAllNotesTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void GetAllNotes_Constructor_GivenNullAllowedStatusesList()
        {
            // act
            var result = Record.Exception(() => new GetAllNotes(new List<NoteType>(), null));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_GivenEmptyAllowedStatusesList()
        {
            // act
            var result = Record.Exception(() => new GetAllNotes(new List<NoteType>(), new List<NoteStatus>()));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_GivenEmptyNoteTypeFilter()
        {
            // act
            var result = Record.Exception(() => new GetAllNotes(null, new List<NoteStatus>()));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_PropertiesShouldBeSet()
        {
            // arrange 
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Returned };
            var noteTypeFilter = new List<NoteType> { NoteType.Evidence };

            // act
            var result = new GetAllNotes(noteTypeFilter, allowedStatuses);

            // assert
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatuses);
            result.NoteTypeFilterList.Should().BeEquivalentTo(noteTypeFilter);
        }
    }
}
