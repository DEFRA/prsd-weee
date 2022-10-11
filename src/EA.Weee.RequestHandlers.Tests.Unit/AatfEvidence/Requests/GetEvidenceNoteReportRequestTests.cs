namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using System;
    using Weee.Requests.AatfEvidence.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNoteReportRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetEvidenceNoteReportRequest_ShouldBeDerivedFromGetEvidenceReportBaseRequest()
        {
            typeof(GetEvidenceNoteReportRequest).Should().BeDerivedFrom<GetEvidenceReportBaseRequest>();
        }

        [Fact]
        public void GetEvidenceNoteReportRequest_GivenRecipientOrganisationId_PropertiesShouldBeSet()
        {
            //arrange
            var recipientOrganisation = TestFixture.Create<Guid?>();
            var tonnageToDisplay = TestFixture.Create<TonnageToDisplayReportEnum>();
            var complianceYear = TestFixture.Create<int>();

            //act
            var request = new GetEvidenceNoteReportRequest(recipientOrganisation,
                null,
                tonnageToDisplay,
                complianceYear);

            //assert
            request.RecipientOrganisationId.Should().Be(recipientOrganisation);
            request.AatfId.Should().Be(null);
            request.TonnageToDisplay.Should().Be(tonnageToDisplay);
            request.ComplianceYear.Should().Be(complianceYear);
            request.InternalRequest.Should().BeFalse();
        }

        [Fact]
        public void GetEvidenceNoteReportRequest_GivenAatfId_PropertiesShouldBeSet()
        {
            //arrange
            var tonnageToDisplay = TestFixture.Create<TonnageToDisplayReportEnum>();
            var complianceYear = TestFixture.Create<int>();
            var aatfId = TestFixture.Create<Guid>();

            //act
            var request = new GetEvidenceNoteReportRequest(null,
                aatfId,
                tonnageToDisplay,
                complianceYear);

            //assert
            request.RecipientOrganisationId.Should().BeNull();
            request.AatfId.Should().Be(aatfId);
            request.TonnageToDisplay.Should().Be(tonnageToDisplay);
            request.ComplianceYear.Should().Be(complianceYear);
            request.InternalRequest.Should().BeFalse();
        }

        [Fact]
        public void GetEvidenceNoteReportRequest_NullAatfAndOrganisation_PropertiesShouldBeSet()
        {
            //arrange
            var tonnageToDisplay = TestFixture.Create<TonnageToDisplayReportEnum>();
            var complianceYear = TestFixture.Create<int>();
            
            //act
            var request = new GetEvidenceNoteReportRequest(null,
                null,
                tonnageToDisplay,
                complianceYear);

            //assert
            request.RecipientOrganisationId.Should().BeNull();
            request.AatfId.Should().BeNull();
            request.TonnageToDisplay.Should().Be(tonnageToDisplay);
            request.ComplianceYear.Should().Be(complianceYear);
            request.InternalRequest.Should().BeTrue();
        }
    }
}
