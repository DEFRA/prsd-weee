namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetAllNotesTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetAllNotes_Constructor_GivenNullAllowedStatusesList()
        {
            // act
            var result = Record.Exception(() => new GetAllNotesInternal(new List<NoteType>(), null, SystemTime.Now.Year, TestFixture.Create<int>(), TestFixture.Create<int>()));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_GivenEmptyAllowedStatusesList()
        {
            // act
            var result = Record.Exception(() => new GetAllNotesInternal(new List<NoteType>(), new List<NoteStatus>(), SystemTime.Now.Year, TestFixture.Create<int>(), TestFixture.Create<int>()));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_GivenEmptyNoteTypeFilter()
        {
            // act
            var result = Record.Exception(() => new GetAllNotesInternal(null, new List<NoteStatus>(), 
                SystemTime.Now.Year, TestFixture.Create<int>(), TestFixture.Create<int>()));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetAllNotes_Constructor_GivenPageSizeIsNotGreaterThanZero(int pageSize)
        {
            // act
            var result = Record.Exception(() => new GetAllNotesInternal(null, new List<NoteStatus>(),
                SystemTime.Now.Year, 1, pageSize));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetAllNotes_Constructor_GivenPageNumberIsNotGreaterThanZero(int pageNumber)
        {
            // act
            var result = Record.Exception(() => new GetAllNotesInternal(null, new List<NoteStatus>(),
                SystemTime.Now.Year, pageNumber, 1));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAllNotes_Constructor_PropertiesShouldBeSet()
        {
            // arrange 
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Returned };
            var noteTypeFilter = new List<NoteType> { NoteType.Evidence };
            var selectedComplianceYear = TestFixture.Create<int>();
            const int pageSize = 1;
            const int pageNumber = 2;

            // act
            var result = new GetAllNotesInternal(noteTypeFilter, allowedStatuses, selectedComplianceYear, pageNumber, pageSize);

            // assert
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatuses);
            result.NoteTypeFilterList.Should().BeEquivalentTo(noteTypeFilter);
            result.ComplianceYear.Should().Be(selectedComplianceYear);
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
        }
    }
}
