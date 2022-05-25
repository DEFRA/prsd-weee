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

    public class GetEvidenceNotesByOrganisationRequestTests
    {
        private readonly Fixture fixture;

        public GetEvidenceNotesByOrganisationRequestTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_ShouldBeDerivedFromEvidenceNoteFilterBaseRequestAsBase()
        {
            typeof(GetEvidenceNotesByOrganisationRequest).Should().BeDerivedFrom<EvidenceNoteFilterBaseRequest>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenOrganisationIdIsDefaultValue_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(Guid.Empty, fixture.CreateMany<NoteStatus>().ToList(), 2022));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenNoteStatusListIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), null, 2022));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenNoteStatusListEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), new List<NoteStatus>(), 2022));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetEvidenceNotesByOrganisationRequest_GivenNComplianceYearIsNotGreaterThanZero_ArgumentOutOfRangeExceptionExpected(short complianceYear)
        {
            //act
            var exception = Record.Exception(() =>
                new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), fixture.CreateMany<NoteStatus>().ToList(), complianceYear));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var statusList = fixture.CreateMany<NoteStatus>().ToList();
            var complianceYear = fixture.Create<short>();

            //act
            var result = new GetEvidenceNotesByOrganisationRequest(organisationId, statusList, complianceYear);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.AllowedStatuses.Should().BeEquivalentTo(statusList);
            result.ComplianceYear.Should().Be(complianceYear);
        }
    }
}
