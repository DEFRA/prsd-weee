namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Web.RazorHelpers;
    using Xunit;

    public class GdsPanelUnitTests
    {
        private readonly WeeeGds<TestModel> htmlHelper;

        public GdsPanelUnitTests()
        {
            htmlHelper = new WeeeGds<TestModel>(A.Fake<WebViewPage<TestModel>>());
        }

        [Fact]
        public void GivenGDSPanel_WithHeaderText_HtmlShouldBeValid()
        {
            var control = htmlHelper.Panel("display text", "header text");

            control.ToString().Should().Be(@"<div class=""govuk-panel govuk-panel--confirmation""><h1 class=""govuk-panel__title"">header text</h1><div class=""govuk-panel__body"">display text</div></div>");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void GivenGDSPanel_WithNoHeaderText_HtmlShouldBeValid(string headerText)
        {
            var control = htmlHelper.Panel("display text", headerText);

            control.ToString().Should().Be(@"<div class=""govuk-panel govuk-panel--confirmation""><div class=""govuk-panel__body"">display text</div></div>");
        }
    }
}
