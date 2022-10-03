namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using AutoFixture;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using Xunit;

    public class GetAllAatfsForComplianceYearRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetAllAatfsForComplianceYearRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetAllAatfsForComplianceYearRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetAllAatfsForComplianceYearRequest_GivenComplianceYear_PropertiesShouldBeSet()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            //act
            var result = new GetAllAatfsForComplianceYearRequest(complianceYear);

            //assert
            result.ComplianceYear.Should().Be(complianceYear);
        }
    }
}
