namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class EditRegisteredCompanyOrganisationDetailsViewModelTests
    {
        [Fact]
        public void GivenModel_CompanyNameShouldHaveRequiredFieldAttribute()
        {
            typeof(EditRegisteredCompanyOrganisationDetailsViewModel).GetProperty("CompanyName")
                .Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void GivenModel_CompanyNameShouldHaveDisplayNameAttribute()
        {
        }
    }
}
