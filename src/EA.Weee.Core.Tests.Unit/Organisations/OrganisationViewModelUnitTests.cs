namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
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

        [Fact]
        public void EntityType_ShouldBeSettable()
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel();
            const EntityType entityType = EntityType.Aatf;

            // Act
            viewModel.EntityType = entityType;

            // Assert
            viewModel.EntityType.Should().Be(entityType);
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
        public void CompaniesRegistrationNumber_ShouldHaveRequiredAndStringLengthAttributes()
        {
            // Act
            var property = typeof(OrganisationViewModel).GetProperty("CompaniesRegistrationNumber");
            var stringLengthAttr = property.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;

            // Assert
            stringLengthAttr.Should().NotBeNull()
                .And.Subject.As<StringLengthAttribute>().Should().Match<StringLengthAttribute>(attr =>
                    attr.MinimumLength == 7 &&
                    attr.MaximumLength == EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber &&
                    attr.ErrorMessage == "The Company registration number should be 7 to 15 characters long");
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
                    CountryId = UkCountryList.ValidCountryIds.ElementAt(0),
                    Postcode = "SW1A 1AA"
                }
            };

            // Act
            var validationResults = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("123456")] // Too short
        [InlineData("1234567890123456")] // Too long
        public void CompaniesRegistrationNumber_ShouldRejectInvalidLengths(string crn)
        {
            // Arrange
            var viewModel = new TestOrganisationViewModel
            {
                CompaniesRegistrationNumber = crn
            };

            // Act
            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(OrganisationViewModel.CompaniesRegistrationNumber)));
        }
    }
}