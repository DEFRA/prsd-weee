namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using EA.Weee.Requests.AatfEvidence;
    using FluentAssertions;
    using System;
    using Xunit;

    public class GetAatfNotesRequestTests
    {
        private readonly Guid organisationId;
        private readonly Guid aatfId;

        public GetAatfNotesRequestTests()
        {
            var fixture = new Fixture();
            organisationId = fixture.Create<Guid>();
            aatfId = fixture.Create<Guid>();
            fixture.Create<string>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyOrganisationArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(Guid.Empty, aatfId));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyAatfArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, Guid.Empty));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenDraftEvidenceNoteValues_PropertiesShouldBeSet()
        {
            // act
            var result = new GetAatfNotesRequest(organisationId, aatfId);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenDraftEvidenceNoteValues_AllowedStatusesSholdBeEmpty()
        {
            // act
            var result = new GetAatfNotesRequest(organisationId, aatfId);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.AllowedStatuses.Should().BeEmpty();
        }
    }
}
