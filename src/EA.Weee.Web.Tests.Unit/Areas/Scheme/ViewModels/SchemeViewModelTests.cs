namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;
    using Web.Areas.Admin.ViewModels.Scheme;
    using Xunit;

    public class SchemeViewModelTests
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
            var model = ValidSchemeViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData("WEE/AB1234CD/SCH")]
        [InlineData("WEE/DE8562FG/SCH")]
        public void ModelWithCorrectApprovalNumber_IsValid(string approvalNumber)
        {
            var model = ValidSchemeViewModel();
            model.ApprovalNumber = approvalNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        private SchemeViewModel ValidSchemeViewModel()
        {
            return new SchemeViewModel
            {
                ApprovalNumber = "WEE/AA0123AA/SCH",
                CompetentAuthorities = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                CompetentAuthorityName = "Any name",
                IbisCustomerReference = "Any value",
                ObligationType = ObligationType.B2B,
                ObligationTypeSelectList = new List<SelectListItem>(),
                SchemeName = "Any value"
            };
        }
    }
}
