namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Web.RazorHelpers;
    using Xunit;

    public class GdsActionLinkUnitTests
    {
        private readonly WeeeGds<TestModel> htmlHelper;

        public GdsActionLinkUnitTests()
        {
            htmlHelper = new WeeeGds<TestModel>(A.Fake<WebViewPage<TestModel>>());
        }

        [Fact]
        public void GivenGDSActionLinkToNewTab_HtmlShouldBeValid()
        {
            var control = htmlHelper.ActionLinkToNewTab("display text", "Url");

            control.ToString().Should().Be(@"<a href=""Url"" target=""_blank"">display text<span class=""govuk-visually-hidden""> This link opens in a new browser window</span></a>");
        }

        [Fact]
        public void GivenGDSActionLinkToNewTabWithAdditionalText_HtmlShouldBeValid()
        {
            var control = htmlHelper.ActionLinkToNewTab("display text", "Url", "additional text");

            control.ToString().Should().Be(@"<a href=""Url"" target=""_blank"">display text<span class=""govuk-visually-hidden"">additional text This link opens in a new browser window</span></a>");
        }
    }
}
