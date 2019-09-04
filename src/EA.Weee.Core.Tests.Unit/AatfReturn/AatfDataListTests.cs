namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Shared;
    using FluentAssertions;
    using Xunit;

    public class AatfDataListTests
    {
        private readonly Fixture fixture;

        public AatfDataListTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void NameWithComplianceYear_GivenNameAndComplianceYear_PropertyShouldBeCorrect()
        {
            var complianceYear = fixture.Create<short>();
            var name = fixture.Create<string>();

            var model = new AatfDataList(fixture.Create<Guid>(),
                name,
                fixture.Create<UKCompetentAuthorityData>(),
                fixture.Create<string>(),
                fixture.Create<AatfStatus>(),
                fixture.Create<OrganisationData>(),
                fixture.Create<FacilityType>(),
                complianceYear,
                fixture.Create<Guid>(),
                fixture.Create<DateTime>());

            model.NameWithComplianceYear.Should().Be($"{name} ({complianceYear})");
        }
    }
}
