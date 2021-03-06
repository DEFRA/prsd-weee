﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class CopyAeViewModelTests
    {
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
            var model = ValidCopyAeViewModel();
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
            var model = ValidCopyAeViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void ModelAeNameIsSet_SiteCopyressNameGetsSetAswell()
        {
            var model = new CopyAeViewModel
            {
                Name = "test name"
            };

            Assert.Equal(model.Name, model.SiteAddressData.Name);
        }

        private CopyAeViewModel ValidCopyAeViewModel()
        {
            return new CopyAeViewModel
            {
                Name = "a name",
                SiteAddressData = new AatfAddressData(),
                ApprovalNumber = "WEE/AA0123AA/AE",
                CompetentAuthoritiesList = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid().ToString(),
                StatusList = Enumeration.GetAll<AatfStatus>(),
                StatusValue = 1,
                SizeList = Enumeration.GetAll<AatfSize>(),
                SizeValue = 1,
                ApprovalDate = new DateTime(1991, 06, 01),
                ComplianceYear = 1991,
                ContactData = new AatfContactData(),
                OrganisationId = Guid.NewGuid(),
                AatfId = Guid.NewGuid()
            };
        }
    }
}
