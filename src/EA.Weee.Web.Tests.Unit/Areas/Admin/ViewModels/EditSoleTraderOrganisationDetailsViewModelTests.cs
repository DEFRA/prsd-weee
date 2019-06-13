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

    public class EditSoleTraderOrganisationDetailsViewModelTests
    {
        private readonly Type modelType;

        public EditSoleTraderOrganisationDetailsViewModelTests()
        {
            modelType = typeof(EditSoleTraderOrganisationDetailsViewModel);
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveRequiredFieldAttribute()
        {
            GetProperty("CompanyName").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldNotHaveRequiredFieldAttribute()
        {
            GetProperty("BusinessTradingName").Should().NotBeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveDisplayNameAttribute()
        {
            var property = GetProperty("CompanyName");
            GetProperty("CompanyName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Equals("Sole trader name");
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveDisplayNameAttribute()
        {
            var property = GetProperty("BusinessTradingName");
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Equals("Business trading name");
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveStringLengthAttribute()
        {
            GetProperty("CompanyName").Should().BeDecoratedWith<StringLengthAttribute>().Which.MaximumLength
                .Equals(CommonMaxFieldLengths.DefaultString);
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveStringLengthAttribute()
        {
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<StringLengthAttribute>().Which.MaximumLength
                .Equals(CommonMaxFieldLengths.DefaultString);
        }

        private PropertyInfo GetProperty(string name)
        {
            return modelType.GetProperty(name);
        }
    }
}
