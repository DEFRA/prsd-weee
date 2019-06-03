namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using FluentAssertions;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Xunit;

    public class EditRegisteredCompanyOrganisationDetailsViewModelTests
    {
        private readonly Type modelType;

        public EditRegisteredCompanyOrganisationDetailsViewModelTests()
        {
            modelType = typeof(EditRegisteredCompanyOrganisationDetailsViewModel);
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveRequiredFieldAttribute()
        {
            GetProperty("CompanyName").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveDisplayNameAttribute()
        {
            GetProperty("CompanyName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Equals("Company name");
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveStringLengthAttribute()
        {
            GetProperty("CompanyName").Should().BeDecoratedWith<StringLengthAttribute>().Which.MaximumLength
                .Equals(CommonMaxFieldLengths.DefaultString);
        }

        [Fact]
        public void GivenModel_CompaniesRegistrationNumberShouldHaveRequiredFieldAttribute()
        {
            GetProperty("CompaniesRegistrationNumber").Should().BeDecoratedWith<RequiredAttribute>()
                .Which.ErrorMessage.Equals("Enter company registration number (CRN)");
        }

        [Fact]
        public void GivenModel_CompaniesRegistrationNumberShouldHaveStringLengthAttribute()
        {
            GetProperty("CompaniesRegistrationNumber").Should()
                .BeDecoratedWith<StringLengthAttribute>(a => a.MaximumLength.Equals(EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber)
                                                             && a.MinimumLength.Equals(7)
                                                             && a.ErrorMessage.Equals(
                                                                 "The Company registration number should be 7 to 15 characters long"));
        }

        [Fact]
        public void GivenModel_CompaniesRegistrationNumberShouldHaveDisplayNameAttribute()
        {
            GetProperty("CompaniesRegistrationNumber").Should().BeDecoratedWith<DisplayAttribute>().Which.Name.Equals("Company registration number (CRN)");
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveDisplayNameAttribute()
        {
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Equals("Business trading name");
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveStringLengthAttribute()
        {
            GetProperty("BusinessTradingName").Should()
                .BeDecoratedWith<StringLengthAttribute>().Which.MaximumLength.Equals(CommonMaxFieldLengths.DefaultString);
        }

        private PropertyInfo GetProperty(string name)
        {
            return modelType.GetProperty(name);
        }
    }
}
