namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using Core.Helpers;
    using FluentAssertions;
    using Xunit;

    public class WindowHelperTests
    {
        [Fact]
        public void IsDateInComplianceYear_GivenDateInCurrentYear_ShouldReturnTrue()
        {
            //act
            var result = WindowHelper.IsDateInComplianceYear(2020, new DateTime(2020, 1, 1));

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDateInComplianceYear_GivenDateInJanuaryOfNextYear_ShouldReturnTrue()
        {
            //act
            var result = WindowHelper.IsDateInComplianceYear(2020, new DateTime(2021, 1, 31));

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDateInComplianceYear_GivenDateNotInComplianceYear_ShouldReturnFalse()
        {
            //act
            var result = WindowHelper.IsDateInComplianceYear(2020, new DateTime(2021, 2, 1));

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsDateInComplianceYear_GivenDateNotInComplianceYear2_ShouldReturnFalse()
        {
            //act
            var result = WindowHelper.IsDateInComplianceYear(2019, new DateTime(2021, 2, 1));

            //assert
            result.Should().BeFalse();
        }
    }
}
