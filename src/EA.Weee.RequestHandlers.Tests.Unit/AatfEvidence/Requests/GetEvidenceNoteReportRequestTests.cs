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
        public void GetEvidenceNoteReportRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var recipientOrganisation = TestFixture.Create<Guid?>();
            var originatingOrganisation = TestFixture.Create<Guid?>();
            var tonnageToDisplay = TestFixture.Create<TonnageToDisplayReportEnum>();
            var complianceYear = TestFixture.Create<int>();

            //act
            var request = new GetEvidenceNoteReportRequest(recipientOrganisation, originatingOrganisation,
                tonnageToDisplay,
                complianceYear);

            //assert
            request.OriginatorOrganisationId.Should().Be(originatingOrganisation);
            request.RecipientOrganisationId.Should().Be(recipientOrganisation);
            request.TonnageToDisplay.Should().Be(tonnageToDisplay);
            request.ComplianceYear.Should().Be(complianceYear);
        }
    }
}
