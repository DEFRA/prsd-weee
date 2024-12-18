﻿#nullable enable

namespace EA.Weee.Core.Tests.Unit.Validation
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class RequiredWhenUKAttributeTests
    {
        private readonly RequiredWhenUKAttribute attribute = new RequiredWhenUKAttribute("Company registration number");

        public RequiredWhenUKAttributeTests()
        {
        }

        [Theory]
        [InlineData(1, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "Company registration number is required")]
        [InlineData(1, null, "DB83F5AB-E745-49CF-B2CA-23FE391B67A8", "Company registration number is required")]
        [InlineData(1, null, "4209EE95-0882-42F2-9A5D-355B4D89EF30", "Company registration number is required")]
        [InlineData(1, "", "7BFB8717-4226-40F3-BC51-B16FDF42550C", "Company registration number is required")]
        [InlineData(1, null, "", "")]
        [InlineData(2, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        [InlineData(3, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        [InlineData(3, null, "", "")]
        public void Validate_WhenUkCountry_ShouldBeRequired(
            int orgType,
            string? companiesRegistrationNumber,
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
            var validationContext = new ValidationContext(viewModel);

            var result = attribute.GetValidationResult(viewModel.CompaniesRegistrationNumber, new ValidationContext(viewModel));

            // Assert
            if (string.IsNullOrEmpty(errorMessage) == false)
            {
                result.ErrorMessage.Should().Be(errorMessage);
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Theory]
        [InlineData("Create", 1, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "Company registration number is required")]
        [InlineData("Create", 1, "", "184E1785-26B4-4AE4-80D3-AE319B103ACB", "Company registration number is required")]
        [InlineData("Edit", 1, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        [InlineData("Edit", 1, "", "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        [InlineData("Create", 2, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        [InlineData("Create", 3, null, "184E1785-26B4-4AE4-80D3-AE319B103ACB", "")]
        public void Validate_ShouldOnlyRequireCompanyNumberOnCreateForUKRegisteredCompanies(
            string action,
            int orgType,
            string companiesRegistrationNumber,
            string countryId,
            string expectedError)
        {
            // Arrange
            var viewModel = new OrganisationViewModel
            {
                Action = action,
                CompaniesRegistrationNumber = companiesRegistrationNumber,
                OrganisationType = (ExternalOrganisationType)orgType,
                Address = new ExternalAddressData
                {
                    CountryId = Guid.Parse(countryId)
                }
            };

            // Act
            var result = attribute.GetValidationResult(viewModel.CompaniesRegistrationNumber, new ValidationContext(viewModel));

            // Assert
            if (string.IsNullOrEmpty(expectedError))
            {
                result.Should().BeNull();
            }
            else
            {
                result.ErrorMessage.Should().Be(expectedError);
            }
        }
    }
}
