namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.RazorHelpers;
    using Prsd.Core.Web.Mvc.Tests.Helpers;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Xunit;

    public class GdsDropDownUnitTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GivenGDSDropDown_ShouldContainGdsCss(bool autocomplete)
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), autocomplete);

            control.ToString().Should().Contain("class").And.Contain("govuk-select");
        }

        [Fact]
        public void GivenGDSDropDown_WithAutoComplete_ShouldContainAutoCompleteCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), true);

            control.ToString().Should().Contain("class").And.Contain("gds-auto-complete");
        }

        [Theory]
        [InlineData("option label")]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenGDSDropDown_WithAutoComplete_ShouldContainAriaLabelledBy(string optionLabel)
        {
            MvcHtmlString control;
            if (!string.IsNullOrWhiteSpace(optionLabel))
            {
                control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), true);
            }
            else
            {
                control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), optionLabel, true);
            }
            
            control.ToString().Should().Contain("aria-labelledby=\"TopLevel-label\" ");
        }

        [Fact]
        public void GivenGDSDropDown_WithoutAutoComplete_ShouldNotContainAriaLabelledBy()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>());

            control.ToString().Should().NotContain("aria-labelledby=\"TopLevel-label\" ");
        }

        [Fact]
        public void GivenGDSDropDown_WithoutAutoComplete_ShouldNotContainAutoCompleteCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>());

            control.ToString().Should().NotContain("gds-auto-complete");
        }
    }
}
