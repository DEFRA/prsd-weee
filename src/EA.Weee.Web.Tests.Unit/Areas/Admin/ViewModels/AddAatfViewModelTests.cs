﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentAssertions;
    using FluentValidation.Attributes;
    using Xunit;

    public class AddAatfViewModelTests
    {
        [Theory]
        [InlineData("Wee/AB1234CD/SCH")]
        [InlineData("WEE/AB1234CD/sch")]
        [InlineData("WEE/AB1234CD/123")]
        [InlineData("WEE/891234CD/SCH")]
        [InlineData("WEE/AB1DF4CD/SCH")]
        [InlineData("WEE/AB123482/SCH")]
        public void ModelWithIncorrectApprovalNumber_IsInvalid(string approvalNumber)
        {
            AddAatfViewModel model = ValidAddAatfViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData("WEE/AB1234CD/ATF")]
        [InlineData("WEE/DE8562FG/ATF")]
        public void ModelWithCorrectApprovalNumber_IsValid(string approvalNumber)
        {
            AddAatfViewModel model = ValidAddAatfViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void ModelAatfNameIsSet_SiteAddressNameGetsSetAswell()
        {
            AddAatfViewModel model = new AddAatfViewModel();
            model.AatfName = "test name";

            Assert.Equal(model.AatfName, model.SiteAddressData.Name);
        }

        [Fact]
        public void AddAatfViewModel_ClassHasValidatorAttribute()
        {
            var t = typeof(AddAatfViewModel);
            var customAttribute = t.GetCustomAttribute(typeof(ValidatorAttribute)) as FluentValidation.Attributes.ValidatorAttribute;

            customAttribute.ValidatorType.Should().Be(typeof(AatfViewModelValidator));
        }

        private AddAatfViewModel ValidAddAatfViewModel()
        {
            return new AddAatfViewModel
            {
                AatfName = "a name",
                SiteAddressData = new Core.AatfReturn.AatfAddressData(),
                ApprovalNumber = "WEE/AA0123AA/ATF",
                CompetentAuthoritiesList = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                StatusList = Enumeration.GetAll<AatfStatus>(),
                StatusValue = 1,
                SizeList = Enumeration.GetAll<AatfSize>(),
                SizeValue = 1,
                ApprovalDate = DateTime.Now,
                ContactData = new AatfContactData(),
                OrganisationId = Guid.NewGuid()
        };
        }
    }
}
