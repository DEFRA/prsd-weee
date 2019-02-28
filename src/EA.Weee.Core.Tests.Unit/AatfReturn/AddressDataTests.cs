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
        public void AddressData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
        {
            var t = typeof(AddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
