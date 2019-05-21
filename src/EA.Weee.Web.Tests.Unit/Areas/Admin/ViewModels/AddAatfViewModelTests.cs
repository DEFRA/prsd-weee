namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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
                SelectedStatusValue = 1,
                SizeList = Enumeration.GetAll<AatfSize>(),
                SelectedSizeValue = 1,
                ApprovalDate = DateTime.Now,
                ContactData = new AatfContactData(),
                OrganisationId = Guid.NewGuid()
        };
        }
    }
}
