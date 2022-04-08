namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using FluentAssertions;
    using Helpers;
    using RazorHelpers;
    using System.Web.Mvc;
    using ViewModels;
    using Xunit;

    public class GdsTextBoxTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();
        private const string GovUkHalfWidth = "govuk-!-width-one-half";

        [Fact]
        public void GivenGDSTextBox_ShouldContainGdsCssClass()
        {
            var control = htmlHelper.Gds().TextBoxFor(m => m.Nested.Bottom);

            control.ToString().Should().Contain("class").And.Contain("govuk-input");
        }

        [Fact]
        public void GivenGDSTextBox_ShouldContainWidthClass()
        {
            var control = htmlHelper.Gds().TextBoxFor(m => m.Nested.Bottom);

            control.ToString().Should().Contain("class").And.Contain(GovUkHalfWidth);
        }

        [Fact]
        public void GivenGDSTextBox_GivenNotHalfWith_ShouldNotContainWidthClass()
        {
            var control = htmlHelper.Gds().TextBoxFor(m => m.Nested.Bottom, false);

            control.ToString().Should().NotContain(GovUkHalfWidth);
        }

        [Fact]
        public void GivenGDSTextBox_DateFieldWithFormatShouldBeRenderedCorrectly()
        {
            var control = htmlHelper.Gds().TextBoxFor(m => m.Date, false, "{0:yyyy-MM-dd}");

            control.ToString().Should().Be(@"<input class=""form-control govuk-input"" id=""Date"" name=""Date"" type=""text"" value=""0001-01-01"" />");
        }
    }
}
