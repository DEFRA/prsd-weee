namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentAssertions;
    using FluentValidation.Attributes;
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

        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public void Name_NoNameSet_ErrorMessageWithCorrectFacility(FacilityType type)
        {
            var model = CreateValidAatfEditDetailsViewModel();
            model.Name = null;
            model.FacilityType = type;

            ValidationContext validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Count() > 0);
            Assert.Equal(string.Format("Enter name of {0}", type), result[0].ErrorMessage);
        }

        [Fact]
        public void AatfEditDetailsViewModel_ClassHasValidatorAttribute()
        {
            var t = typeof(AatfEditDetailsViewModel);
            var customAttribute = t.GetCustomAttribute(typeof(ValidatorAttribute)) as FluentValidation.Attributes.ValidatorAttribute;

            customAttribute.ValidatorType.Should().Be(typeof(ApprovalDateValidator));
        }

        private AatfEditDetailsViewModel CreateValidAatfEditDetailsViewModel()
        {
            return fixture.Build<AatfEditDetailsViewModel>()
                .With(a => a.StatusList, Enumeration.GetAll<AatfStatus>())
                .With(a => a.StatusValue, AatfStatus.Approved.Value)
                .With(a => a.SizeList, Enumeration.GetAll<AatfSize>())
                .With(a => a.SizeValue, AatfSize.Large.Value)
                .Create();
        }
    }
}
