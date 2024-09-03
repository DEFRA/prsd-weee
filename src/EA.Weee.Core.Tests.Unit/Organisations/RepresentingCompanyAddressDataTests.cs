namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class RepresentingCompanyAddressDataTests
    {
        [Theory]
        [InlineData("Address1")]
        [InlineData("TownOrCity")]
        [InlineData("CountryId")]
        [InlineData("Telephone")]
        [InlineData("Email")]
        public void RepresentingCompanyAddressData_RequiredVariablesShouldHaveRequiredAttribute(string variable)
        {
            var t = typeof(RepresentingCompanyAddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }

        [Theory]
        [InlineData("Address1")]
        [InlineData("Address2")]
        [InlineData("TownOrCity")]
        [InlineData("CountyOrRegion")]
        [InlineData("Postcode")]
        public void RepresentingCompanyAddressData_VariablesShouldHaveMaxLengthAttribute(string variable)
        {
            var t = typeof(RepresentingCompanyAddressData);
            var pi = t.GetProperty(variable);
            var hasAttribute = Attribute.IsDefined(pi, typeof(StringLengthAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void RepresentingCompanyAddressData_EmailShouldHaveEmailAddressAttribute()
        {
            var t = typeof(RepresentingCompanyAddressData);
            var pi = t.GetProperty("Email");
            var hasAttribute = Attribute.IsDefined(pi, typeof(EmailAddressAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]
        public void RepresentingCompanyAddressData_TelephoneShouldHaveGenericPhoneNumberAttribute()
        {
            var t = typeof(RepresentingCompanyAddressData);
            var pi = t.GetProperty("Telephone");
            var hasAttribute = Attribute.IsDefined(pi, typeof(GenericPhoneNumberAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
