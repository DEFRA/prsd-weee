namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class OrganisationTransactionDataTests
    {
        [Fact]
        public void Validate_WithValidData_ReturnsNoValidationErrors()
        {
            var data = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.RegisteredCompany,
                OrganisationViewModel = new OrganisationViewModel()
            };

            var results = data.Validate(new ValidationContext(data));
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("TonnageType")]
        [InlineData("OrganisationType")]
        [InlineData("PreviousRegistration")]
        [InlineData("AuthorisedRepresentative")]
        public void RequiredProperties_ShouldHaveRequiredAttribute(string propertyName)
        {
            typeof(OrganisationTransactionData).GetProperty(propertyName)
                .Should().BeDecoratedWith<RequiredAttribute>();
        }
    }
}