namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetEvidenceNoteByPbsOrganisationRequestTests
    {
        private readonly Fixture fixture;
        private readonly int complianceYear;

        public GetEvidenceNoteByPbsOrganisationRequestTests()
        {
            fixture = new Fixture();
            complianceYear = fixture.Create<int>();
        }

        [Fact]
        public void GetEvidenceNoteByPbsOrganisationRequest_ShouldBeDerivedFromEvidenceNoteFilterBaseRequestAsBase()
        {
            typeof(GetEvidenceNotesByOrganisationRequest).Should().BeDerivedFrom<EvidenceNoteFilterBaseRequest>();
        }

        [Fact]
        public void GetEvidenceNoteByPbsOrganisationRequest_GivenOrganisationIdIsDefaultValue_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(Guid.Empty, fixture.CreateMany<NoteStatus>().ToList(), complianceYear, new List<NoteType>() { NoteType.Evidence }, false, 1, 25));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNoteByPbsOrganisationRequest_GivenNoteStatusListIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), null, complianceYear, new List<NoteType>() { NoteType.Evidence }, false, 1, 25));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetEvidenceNoteByPbsOrganisationRequest_GivenNoteStatusListEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), new List<NoteStatus>(), complianceYear, new List<NoteType>() { NoteType.Evidence }, false, 1, 25));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetEvidenceNoteByPbsOrganisationRequest_GivenNComplianceYearIsNotGreaterThanZero_ArgumentOutOfRangeExceptionExpected(int currentYear)
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), fixture.CreateMany<NoteStatus>().ToList(), currentYear, new List<NoteType>() { NoteType.Evidence }, false, 1, 25));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetEvidenceNoteByPbsOrganisationRequest_GivenValues_PropertiesShouldBeSet(bool transferredOut)
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var statusList = fixture.CreateMany<NoteStatus>().ToList();

            //act
            var result = new GetEvidenceNotesByOrganisationRequest(organisationId, statusList, complianceYear, new List<NoteType>() { NoteType.Evidence }, transferredOut, 1, 25);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.AllowedStatuses.Should().BeEquivalentTo(statusList);
            result.ComplianceYear.Should().Be(complianceYear);
            result.TransferredOut.Should().Be(transferredOut);
        }
    }
}
