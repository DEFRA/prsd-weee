namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using AutoFixture;
    using Core.Shared;
    using FluentAssertions;
    using Requests.Admin.Obligations;
    using System;
    using Weee.Tests.Core;
    using Xunit;

    public class GetObligationSchemeTests : SimpleUnitTestBase
    {
        public GetObligationSchemeTests()
        {
            TestFixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 2022));
        }

        [Fact]
        public void GetSchemeObligation_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var authority = TestFixture.Create<CompetentAuthority>();
            var complianceYear = TestFixture.Create<int>();

            //act
            var request = new GetSchemeObligation(authority, complianceYear);

            //assert
            request.ComplianceYear.Should().Be(complianceYear);
            request.Authority.Should().Be(authority);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetSchemeObligation_GivenYearIsLessThanZero_ArgumentNullExceptionExpected(int complianceYear)
        {
            //act
            var exception = Record.Exception(() => new GetSchemeObligation(TestFixture.Create<CompetentAuthority>(), complianceYear));

            //assert 
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }
    }
}
