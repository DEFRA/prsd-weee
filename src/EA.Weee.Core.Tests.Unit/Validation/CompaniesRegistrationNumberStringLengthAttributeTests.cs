namespace EA.Weee.Core.Tests.Unit.Validation
{
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class CompaniesRegistrationNumberStringLengthAttributeTests
    {
        private readonly List<ValidationResult> validationResults = new List<ValidationResult>();
        private readonly CompaniesRegistrationNumberStringLengthAttribute attribute = new CompaniesRegistrationNumberStringLengthAttribute();

        [Theory]
        [InlineData(1, "456789", "184E1785-26B4-4AE4-80D3-AE319B103ACB", "The company registration number should be 7 to 8 characters long")]
        [InlineData(1, "7897898799", "DB83F5AB-E745-49CF-B2CA-23FE391B67A8", "The company registration number should be 7 to 8 characters long")]
        [InlineData(1, "7897898799", "4209EE95-0882-42F2-9A5D-355B4D89EF30", "The company registration number should be 7 to 8 characters long")]
        [InlineData(1, "7897898799", "7BFB8717-4226-40F3-BC51-B16FDF42550C", "The company registration number should be 7 to 8 characters long")]
        [InlineData(1, "456", "", "The company registration number should be 7 to 15 characters long")]
        [InlineData(2, "456", "184E1785-26B4-4AE4-80D3-AE319B103ACB", "The company registration number should be 7 to 15 characters long")]
        [InlineData(3, "456", "184E1785-26B4-4AE4-80D3-AE319B103ACB", "The company registration number should be 7 to 15 characters long")]
        [InlineData(3, "456", "", "The company registration number should be 7 to 15 characters long")]
        public void Validate_InvalidLength_ShouldReturnError(
            int orgType,
            string companiesRegistrationNumber,
            string countryId,
            string errorMessage)
        {
            // Arrange
            var viewModel = new OrganisationViewModel
            {
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                OrganisationType = (Core.Organisations.ExternalOrganisationType)orgType,
                Address = new Core.Organisations.ExternalAddressData
                {
                    CountryId = countryId == string.Empty ? Guid.NewGuid() : Guid.Parse(countryId)
                }
            };

            // Act
            var result = attribute.GetValidationResult(viewModel.CompaniesRegistrationNumber, new ValidationContext(viewModel));

            // Assert
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Theory]
        [InlineData(1, "4567898", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData(1, "7894561", "DB83F5AB-E745-49CF-B2CA-23FE391B67A8")]
        [InlineData(1, "45678941", "4209EE95-0882-42F2-9A5D-355B4D89EF30")]
        [InlineData(1, "78945698", "7BFB8717-4226-40F3-BC51-B16FDF42550C")]
        [InlineData(1, "78945658", "")]
        [InlineData(2, "4567894", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData(3, "12345678", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData(3, "4567894", "")]
        public void Validate_ValidLength_ShouldPass(
           int orgType,
           string companiesRegistrationNumber,
           string countryId)
        {
            // Arrange
            var viewModel = new OrganisationViewModel
            {
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                OrganisationType = (Core.Organisations.ExternalOrganisationType)orgType,
                Address = new Core.Organisations.ExternalAddressData
                {
                    CountryId = countryId == string.Empty ? Guid.NewGuid() : Guid.Parse(countryId)
                }
            };

            // Act
            var result = attribute.GetValidationResult(viewModel.CompaniesRegistrationNumber, new ValidationContext(viewModel));

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Validate_NullEmptyOrWhitespace_ShouldPass(string companiesRegistrationNumber)
        {
            // Arrange
            var viewModel = new OrganisationViewModel
            {
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                OrganisationType = Core.Organisations.ExternalOrganisationType.RegisteredCompany,
                Address = new Core.Organisations.ExternalAddressData
                {
                    CountryId = Guid.NewGuid()
                }
            };

            // Act
            var result = attribute.GetValidationResult(viewModel.CompaniesRegistrationNumber, new ValidationContext(viewModel));

            // Assert
            result.Should().BeNull();
        }
    }
}