﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class AeEditDetailsViewModelTests
    {
        private readonly Fixture fixture;

        public AeEditDetailsViewModelTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData("Wee/AB1234CD/SCH")]
        [InlineData("WEE/AB1234CD/sch")]
        [InlineData("WEE/AB1234CD/123")]
        [InlineData("WEE/891234CD/SCH")]
        [InlineData("WEE/AB1DF4CD/SCH")]
        [InlineData("WEE/AB123482/SCH")]
        [InlineData("WEE/AB1234CD/ATF")]
        public void ModelWithIncorrectApprovalNumber_IsInvalid(string approvalNumber)
        {
            var model = CreateValidAeEditDetailsViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData("WEE/AB1234CD/AE")]
        [InlineData("WEE/DE8562FG/EXP")]
        public void ModelWithCorrectApprovalNumber_IsValid(string approvalNumber)
        {
            var model = CreateValidAeEditDetailsViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void ManageAesViewModel_RequiredVariableShouldHaveRequiredAttribute()
        {
            var requiredProperties = new List<string>
            {
                "Name",
                "ApprovalNumber",
                "CompetentAuthorityId",
                "ApprovalDate",
                "ComplianceYear",
                "StatusValue",
                "SizeValue"
            };

            foreach (var property in typeof(AatfEditDetailsViewModel).GetProperties())
            {
                Attribute.IsDefined(property, typeof(RequiredAttribute)).Should().Be(requiredProperties.Contains(property.Name), "{0} should be required", property.Name);
                requiredProperties.Remove(property.Name);
            }

            requiredProperties.Should().BeEmpty();
        }

        [Fact]
        public void Name_NameSet_SiteAddressNameGetsSet()
        {
            var model = CreateValidAeEditDetailsViewModel();
            model.Name = "AE Name";

            Assert.Equal(model.Name, model.SiteAddressData.Name);
        }

        [Fact]
        public void Name_NoNameSet_ErrorMessageWithCorrectFacility()
        {
            var model = CreateValidAeEditDetailsViewModel();
            model.Name = null;

            var validationContext = new ValidationContext(model, null, null);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, results, true);

            Assert.False(isValid);
            Assert.Equal("Enter name of AE", results[0].ErrorMessage);
        }

        private AeEditDetailsViewModel CreateValidAeEditDetailsViewModel()
        {
            return fixture.Build<AeEditDetailsViewModel>()
                .With(a => a.StatusList, Enumeration.GetAll<AatfStatus>())
                .With(a => a.StatusValue, AatfStatus.Approved.Value)
                .With(a => a.SizeList, Enumeration.GetAll<AatfSize>())
                .With(a => a.SizeValue, AatfSize.Large.Value)
                .With(a => a.ApprovalDate, new DateTime(1991, 06, 01))
                .With(a => a.ComplianceYear, 1991)
                .Create();
        }
    }
}
