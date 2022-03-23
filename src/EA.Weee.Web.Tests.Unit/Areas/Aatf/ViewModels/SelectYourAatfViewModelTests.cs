namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Xunit;

    public class SelectYourAatfViewModelTests
    {
        [Fact]
        public void Selected_NoSelected_ShowsErrorMessage()
        {
            var model = new SelectYourAatfViewModel();

            var validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Any());
            result[0].ErrorMessage.Should().Be($"Select the site you would like to manage");
            model.ModelValidated.Should().BeTrue();
        }

        [Fact]
        public void Selected_IsSelected_ShouldBeValid()
        {
            var model = new SelectYourAatfViewModel()
            {
                SelectedId = Guid.NewGuid()
            };

            var validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            result.Should().BeEmpty();
            model.ModelValidated.Should().BeTrue();
        }

        [Fact]
        public void SelectYourAatf_SelectedId_ShouldBeDecoratedWith_Attributes()
        {
            typeof(SelectYourAatfViewModel).GetProperty("SelectedId")
            .Should()
            .BeDecoratedWith<RequiredAttribute>(t =>
            t.ErrorMessage.Equals("Select the site you would like to manage"));

            typeof(SelectYourAatfViewModel).GetProperty("SelectedId")
            .Should()
            .BeDecoratedWith<DisplayNameAttribute>(t =>
            t.DisplayName.Equals("Which site would you like to manage evidence notes for?"));
        }
    }
}
