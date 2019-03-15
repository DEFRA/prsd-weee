namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class AddressDataTests
    {
        [Theory]
        [InlineData("Name")]
        [InlineData("Address1")]
        [InlineData("TownOrCity")]
        [InlineData("CountryId")]
        public void SiteAddressData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
        {
            var t = typeof(SiteAddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }

        [Theory]
        [InlineData("Name")]
        [InlineData("Address1")]
        [InlineData("Address2")]
        [InlineData("TownOrCity")]
        [InlineData("CountyOrRegion")]
        [InlineData("Postcode")]
        public void SiteAddressData_VariablesShouldHaveMaxLengthAttribute(string variable)
        {
            var t = typeof(SiteAddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(StringLengthAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
