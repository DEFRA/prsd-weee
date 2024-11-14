namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Xunit;

    public class OrganisationViewModelTests
    {
        private class TestOrganisationViewModel : OrganisationViewModel
        {
            public override string CompanyName { get; set; }
            public override string BusinessTradingName { get; set; }
        }

        [Fact]
        public void CompanyName_ShouldBeSettable()
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel();
            const string companyName = "Test Company";

            // Act
            viewModel.CompanyName = companyName;

            // Assert
            viewModel.CompanyName.Should().Be(companyName);
        }

        [Fact]
        public void BusinessTradingName_ShouldBeSettable()
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel();
            const string tradingName = "Test Trading Name";

            // Act
            viewModel.BusinessTradingName = tradingName;

            // Assert
            viewModel.BusinessTradingName.Should().Be(tradingName);
        }

        [Fact]
        public void Address_ShouldBeInitialized()
        {
            // Arrange & Act
            var viewModel = new TestOrganisationViewModel();

            // Assert
            viewModel.Address.Should().NotBeNull()
                .And.BeOfType<ExternalAddressData>();
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("123456789012345")]
        public void CompaniesRegistrationNumber_ShouldAcceptValidLengths(string crn)
        {
            // Arrange & Act
            var viewModel = new TestOrganisationViewModel
            {
                CompaniesRegistrationNumber = crn
            };

            // Assert
            viewModel.CompaniesRegistrationNumber.Should().Be(crn);
        }

        [Fact]
        public void EEEBrandNames_ShouldBeSettable()
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel();
            const string brandNames = "Brand1, Brand2, Brand3";

            // Act
            viewModel.EEEBrandNames = brandNames;

            // Assert
            viewModel.EEEBrandNames.Should().Be(brandNames);
        }

        [Fact]
        public void ValidationMessageDisplayOrder_ShouldContainAllExpectedFields()
        {
            // Arrange
            var expectedFields = new[]
            {
                "Address.CountryId",
                nameof(OrganisationViewModel.CompaniesRegistrationNumber),
                nameof(OrganisationViewModel.ProducerRegistrationNumber),
                nameof(OrganisationViewModel.CompanyName),
                nameof(OrganisationViewModel.BusinessTradingName),
                "Address.WebsiteAddress",
                "Address.Address1",
                "Address.Address2",
                "Address.TownOrCity",
                "Address.CountyOrRegion",
                "Address.Postcode"
            };

            // Act
            var actualOrder = OrganisationViewModel.ValidationMessageDisplayOrder.ToList();

            // Assert
            actualOrder.Should().BeEquivalentTo(expectedFields, options => options.WithStrictOrdering());
        }

        [Fact]
        public void CompanyName_ShouldHaveStringLengthAttribute()
        {
            // Act
            var property = typeof(OrganisationViewModel).GetProperty("CompanyName");
            var attribute = property.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;

            // Assert
            attribute.Should().NotBeNull();
            attribute.MaximumLength.Should().Be(CommonMaxFieldLengths.DefaultString);
        }

        [Fact]
        public void BusinessTradingName_ShouldHaveStringLengthAndDisplayNameAttributes()
        {
            // Act
            var property = typeof(OrganisationViewModel).GetProperty("BusinessTradingName");
            var stringLengthAttr = property.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;
            var displayNameAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).FirstOrDefault() as System.ComponentModel.DisplayNameAttribute;

            // Assert
            stringLengthAttr.Should().NotBeNull()
                .And.Subject.As<StringLengthAttribute>().MaximumLength.Should().Be(CommonMaxFieldLengths.DefaultString);

            displayNameAttr.Should().NotBeNull()
                .And.Subject.As<System.ComponentModel.DisplayNameAttribute>().DisplayName.Should().Be("Business trading name");
        }

        [Fact]
        public void EEEBrandNames_ShouldHaveRequiredAndStringLengthAttributes()
        {
            // Act
            var property = typeof(OrganisationViewModel).GetProperty("EEEBrandNames");
            var stringLengthAttr = property.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;
            var displayNameAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).FirstOrDefault() as System.ComponentModel.DisplayNameAttribute;

            // Assert
            stringLengthAttr.Should().NotBeNull()
                .And.Subject.As<StringLengthAttribute>().MaximumLength.Should().Be(CommonMaxFieldLengths.DefaultString);

            displayNameAttr.Should().NotBeNull()
                .And.Subject.As<System.ComponentModel.DisplayNameAttribute>().DisplayName.Should().Be("If you are registering as an authorised representative of a non-UK established organisation, enter the brands they place on the market.");
        }

        [Fact]
        public void Validate_ShouldCallExternalAddressValidator()
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel
            {
                Address = new ExternalAddressData
                {
                    CountryId = UkCountry.ValidIds.ElementAt(0),
                    Postcode = "SW1A 1AA"
                }
            };

            // Act
            var validationResults = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void EEEBrandNames_HasRequiredAttribute()
        {
            var t = typeof(OrganisationViewModel);
            var pi = t.GetProperty("EEEBrandNames");
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }

        [Fact]

        public void CompaniesRegistrationNumber_HasCorrectAttributes()
        {
            var t = typeof(OrganisationViewModel);
            var pi = t.GetProperty("CompaniesRegistrationNumber");
            var hasRequiredAttribute = Attribute.IsDefined(pi, typeof(EA.Weee.Core.Validation.RequiredWhenUKAttribute));
            var hasCompaniesRegistrationNumberStringLengthAttribute = Attribute.IsDefined(pi, typeof(EA.Weee.Core.Validation.CompaniesRegistrationNumberStringLengthAttribute));

            hasRequiredAttribute.Should().Be(true);
            hasCompaniesRegistrationNumberStringLengthAttribute.Should().Be(true);
        }

        [Fact]
        public void HasPaid_DefaultValueShouldBeFalse()
        {
            var viewModel = new TestOrganisationViewModel();
            viewModel.HasPaid.Should().BeFalse();
        }

        [Fact]
        public void ProducerRegistrationNumber_ShouldHaveStringLengthAttribute()
        {
            var property = typeof(OrganisationViewModel).GetProperty("ProducerRegistrationNumber");
            var attribute = property.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;

            attribute.Should().NotBeNull();
            attribute.MaximumLength.Should().Be(CommonMaxFieldLengths.ProducerRegistrationNumber);
        }

        [Fact]
        public void Validate_WhenNonUKCountryAndHasAuthorisedRepresentitive_ShouldReturnValidationError()
        {
            var viewModel = new TestOrganisationViewModel
            {
                HasAuthorisedRepresentitive = true,
                Address = new ExternalAddressData
                {
                    CountryId = Guid.NewGuid(), // Non-UK country
                    Postcode = "12345"
                }
            };

            var validationResults = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            validationResults.Should().HaveCount(1);
            validationResults[0].ErrorMessage.Should().Be("Selected country must be a UK country");
        }

        [Theory]
        [InlineData(ExternalOrganisationType.RegisteredCompany, typeof(RegisteredCompanyDetailsViewModel))]
        [InlineData(ExternalOrganisationType.Partnership, typeof(PartnershipDetailsViewModel))]
        [InlineData(ExternalOrganisationType.SoleTrader, typeof(SoleTraderDetailsViewModel))]
        [InlineData(null, typeof(RegisteredCompanyDetailsViewModel))]
        public void CastToSpecificViewModel_ShouldReturnCorrectType(ExternalOrganisationType? orgType, Type expectedType)
        {
            var viewModel = new TestOrganisationViewModel { OrganisationType = orgType };

            var result = viewModel.CastToSpecificViewModel(viewModel);

            result.Should().BeOfType(expectedType);
        }
    }
}