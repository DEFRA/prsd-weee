﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Web.Areas.Admin.ViewModels.Organisation;
    using FluentAssertions;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Xunit;

    public class EditPartnershipOrganisationDetailsViewModelTests
    {
        private readonly Type modelType;

        public EditPartnershipOrganisationDetailsViewModelTests()
        {
            modelType = typeof(EditPartnershipOrganisationDetailsViewModel);
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveRequiredFieldAttribute()
        {
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveDisplayNameAttribute()
        {
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<DisplayNameAttribute>().Which.DisplayName.Should().Be("Business trading name");
        }

        [Fact]
        public void GivenModel_BusinessTradingNameShouldHaveStringLengthAttribute()
        {
            GetProperty("BusinessTradingName").Should().BeDecoratedWith<StringLengthAttribute>().Which.MaximumLength
                .Should().Be(CommonMaxFieldLengths.DefaultString);
        }

        private PropertyInfo GetProperty(string name)
        {
            return modelType.GetProperty(name);
        }
    }
}
