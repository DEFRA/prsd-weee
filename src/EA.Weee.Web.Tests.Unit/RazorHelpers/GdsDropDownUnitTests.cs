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
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void GivenGDSDropDown_ShouldContainGdsCss(bool autocomplete, bool useHalfWidth)
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), autocomplete, useHalfWidth);

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
                control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), optionLabel, true, true);
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

        [Fact]
        public void GivenGDSDropDown_WithoutHalfWidth_ShouldNotContainHalfWidthCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), useHalfWidth: false);

            control.ToString().Should().NotContain("govuk-!-width-one-half");
        }

        [Fact]
        public void GivenGDSDropDown_WithHalfWidth_ShouldContainHalfWidthCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), useHalfWidth: true);

            control.ToString().Should().Contain("class").And.Contain("govuk-!-width-one-half");
        }

        [Fact]
        public void GivenGDSDropDownAlt_WithoutHalfWidth_ShouldNotContainHalfWidthCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), new object(), useHalfWidth: false);

            control.ToString().Should().NotContain("govuk-!-width-one-half");
        }

        [Fact]
        public void GivenGDSDropDownAlt_WithHalfWidth_ShouldContainHalfWidthCss()
        {
            var control = htmlHelper.Gds().DropDownListFor(m => m.TopLevel, new List<SelectListItem>(), new object(), useHalfWidth: true);

            control.ToString().Should().Contain("class").And.Contain("govuk-!-width-one-half");
        }
    }
}
