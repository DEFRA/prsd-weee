namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using Xunit;

    public class AatfEditDetailsViewModelTests
    {
        private readonly Fixture fixture;

        public AatfEditDetailsViewModelTests()
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
        [InlineData("WEE/AB1234CD/AE")]
        [InlineData("WEE/AB1234CD/EXP")]
        public void ModelWithIncorrectApprovalNumber_IsInvalid(string approvalNumber)
        {
            var model = CreateValidAatfEditDetailsViewModel();
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
            var model = CreateValidAatfEditDetailsViewModel();
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
            var model = CreateValidAatfEditDetailsViewModel();
            model.Name = "AATF Name";

            Assert.Equal(model.Name, model.SiteAddressData.Name);
        }

        [Fact]
        public void Name_NoNameSet_ErrorMessageWithCorrectFacility()
        {
            var model = CreateValidAatfEditDetailsViewModel();
            model.Name = null;

            var validationContext = new ValidationContext(model, null, null);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, results, true);

            Assert.False(isValid);
            Assert.Equal("Enter name of AATF", results[0].ErrorMessage);
        }

        private AatfEditDetailsViewModel CreateValidAatfEditDetailsViewModel()
        {
            return fixture.Build<AatfEditDetailsViewModel>()
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
