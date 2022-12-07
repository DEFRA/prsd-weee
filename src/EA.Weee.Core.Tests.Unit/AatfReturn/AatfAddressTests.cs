namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class AatfAddressTests
    {
        [Theory]
        [InlineData("Name")]
        [InlineData("Address1")]
        [InlineData("TownOrCity")]
        [InlineData("CountryId")]
        public void AatfAddressData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
        {
            var t = typeof(AatfAddressData);
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
        public void AatfAddressData_VariablesShouldHaveMaxLengthAttribute(string variable)
        {
            var t = typeof(AatfAddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(StringLengthAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void AatfAddressData_SiteName_ErrorMessage()
        {
            Type t = typeof(AatfAddressData);
            PropertyInfo pi = t.GetProperty("Name");

            Attribute[] attrs = Attribute.GetCustomAttributes(pi);

            RequiredAttribute attr = attrs.FirstOrDefault(p => p is RequiredAttribute) as RequiredAttribute;

            Assert.NotNull(attr);
            Assert.Equal("Enter ATF site name", attr.ErrorMessage);
        }
    }
}
