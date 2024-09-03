namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Core.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class OrganisationTransactionDataTests
    {
        [Fact]
        public void GetAddressData_WithValidOrganisationType_ReturnsCorrectAddress()
        {
            // Arrange
            var data = new OrganisationTransactionData();
            var address = A.Fake<ExternalAddressData>();

            data.OrganisationType = ExternalOrganisationType.RegisteredCompany;
            data.RegisteredCompanyDetailsViewModel = new RegisteredCompanyDetailsViewModel { Address = address };
            data.GetAddressData().Should().Be(address);

            data.OrganisationType = ExternalOrganisationType.Partnership;
            data.PartnershipDetailsViewModel = new PartnershipDetailsViewModel { Address = address };
            data.GetAddressData().Should().Be(address);

            data.OrganisationType = ExternalOrganisationType.SoleTrader;
            data.SoleTraderDetailsViewModel = new SoleTraderDetailsViewModel { Address = address };
            data.GetAddressData().Should().Be(address);
        }

        [Fact]
        public void GetAddressData_WithInvalidOrganisationType_ThrowsInvalidOperationException()
        {
            var data = new OrganisationTransactionData { OrganisationType = (ExternalOrganisationType)999 };
            data.Invoking(d => d.GetAddressData())
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid organisation type.");
        }

        [Fact]
        public void GetBrandNames_WithValidOrganisationType_ReturnsCorrectBrandNames()
        {
            var data = new OrganisationTransactionData();
            const string brandNames = "Fake Brand Names";

            data.OrganisationType = ExternalOrganisationType.RegisteredCompany;
            data.RegisteredCompanyDetailsViewModel = new RegisteredCompanyDetailsViewModel { EEEBrandNames = brandNames };
            data.GetBrandNames().Should().Be(brandNames);

            data.OrganisationType = ExternalOrganisationType.Partnership;
            data.PartnershipDetailsViewModel = new PartnershipDetailsViewModel { EEEBrandNames = brandNames };
            data.GetBrandNames().Should().Be(brandNames);

            data.OrganisationType = ExternalOrganisationType.SoleTrader;
            data.SoleTraderDetailsViewModel = new SoleTraderDetailsViewModel { EEEBrandNames = brandNames };
            data.GetBrandNames().Should().Be(brandNames);
        }

        [Fact]
        public void GetBrandNames_WithInvalidOrganisationType_ThrowsInvalidOperationException()
        {
            var data = new OrganisationTransactionData { OrganisationType = (ExternalOrganisationType)999 };
            data.Invoking(d => d.GetBrandNames())
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid organisation type.");
        }

        [Fact]
        public void Validate_WithValidData_ReturnsNoValidationErrors()
        {
            var data = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.RegisteredCompany,
                RegisteredCompanyDetailsViewModel = new RegisteredCompanyDetailsViewModel()
            };

            var results = data.Validate(new ValidationContext(data));
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(ExternalOrganisationType.Partnership, "Partnership details are required")]
        [InlineData(ExternalOrganisationType.RegisteredCompany, "Registered company details are required")]
        [InlineData(ExternalOrganisationType.SoleTrader, "Sole trader details are required")]
        public void Validate_WithMissingDetails_ReturnsValidationError(ExternalOrganisationType organisationType, string expectedErrorMessage)
        {
            var data = new OrganisationTransactionData
            {
                OrganisationType = organisationType
            };

            var results = data.Validate(new ValidationContext(data));

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be(expectedErrorMessage);
        }

        [Fact]
        public void Validate_WithInvalidOrganisationType_ThrowsInvalidOperationException()
        {
            var data = new OrganisationTransactionData { OrganisationType = (ExternalOrganisationType)999 };
            data.Invoking(d => d.Validate(new ValidationContext(data)))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid organisation type.");
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