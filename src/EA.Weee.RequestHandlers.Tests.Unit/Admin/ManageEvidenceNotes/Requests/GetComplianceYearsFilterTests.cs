namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public class GetComplianceYearsFilterTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetComplianceYearsFilter_Constructor_GivenNullList_ShouldThrowArgumentNullException()
        {
            // act
            var result = Record.Exception(() => new GetComplianceYearsFilter(null));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetComplianceYearsFilter_Constructor_GivenEmptyList_ShouldThrowArgumentNullException()
        {
            // act
            var result = Record.Exception(() => new GetComplianceYearsFilter(new List<NoteStatus>()));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetComplianceYearsFilter_Constructor_PropertiesShouldBeSet()
        {
            // arrange 
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Returned, NoteStatus.Submitted };
           
            // act
            var result = new GetComplianceYearsFilter(allowedStatuses);

            // assert
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatuses);
        }
    }
}
