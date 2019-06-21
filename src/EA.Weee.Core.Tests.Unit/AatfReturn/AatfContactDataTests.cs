namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Validation;
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
        public void AatContactData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
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
        public void AatContactData_VariablesShouldHaveMaxLengthAttribute(string variable)
        {
            var t = typeof(AatfContactData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(StringLengthAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void AatContactData_TelephonePropertyShouldHaveTelephoneProperty()
        {
            var t = typeof(AatfContactData);
            var pi = t.GetProperty("Telephone");
            var hasAttribute = Attribute.IsDefined(pi, typeof(GenericPhoneNumberAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void AatContactData_EmailPropertyShouldHaveEmailProperty()
        {
            var t = typeof(AatfContactData);
            var pi = t.GetProperty("Email");
            var hasAttribute = Attribute.IsDefined(pi, typeof(EmailAddressAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
