namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class AatfContactDataTests
    {
        [Theory]
        [InlineData("FirstName")]
        [InlineData("LastName")]
        [InlineData("Position")]
        [InlineData("Telephone")]
        [InlineData("Email")]
        public void AatfAddressData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
        {
            var t = typeof(AatfContactData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }

        [Theory]
        [InlineData("FirstName")]
        [InlineData("LastName")]
        [InlineData("Position")]
        [InlineData("Telephone")]
        [InlineData("Email")]
        public void AatfAddressData_VariablesShouldHaveMaxLengthAttribute(string variable)
        {
            var t = typeof(AatfContactData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(StringLengthAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
