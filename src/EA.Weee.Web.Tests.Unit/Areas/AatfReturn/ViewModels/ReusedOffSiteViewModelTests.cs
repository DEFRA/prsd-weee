namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReusedOffSiteViewModelTests
    {
        [Fact]
        public void ReusedOffSiteViewModel_GivenSelectedValueIsNull_IsInvalid()
        {
            var viewModel = new ReusedOffSiteViewModel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), A.Dummy<String>(), null);

            var context = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public void ReusedOffSiteViewModel_GivenSelectedValueIsYesOrNo_IsValid(string selectedValue)
        {
            var viewModel = new ReusedOffSiteViewModel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), A.Dummy<String>(), selectedValue);

            var context = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void ReusedOffSiteViewModel_SelectedValueVariableShouldHaveRequiredAttribute()
        {
            var t = typeof(ReusedOffSiteViewModel);
            var pi = t.GetProperty("SelectedValue");
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
