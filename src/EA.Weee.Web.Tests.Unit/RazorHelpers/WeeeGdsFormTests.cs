namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.Tests.Helpers;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Web.RazorHelpers;
    using Xunit;

    public class WeeeGdsFormTests
    {
        private readonly WeeeGds<TestModel> htmlHelper;

        public WeeeGdsFormTests()
        {
            htmlHelper = new WeeeGds<TestModel>(A.Fake<WebViewPage<TestModel>>());
        }

        [Fact]
        public void Button_ShouldRenderCorrectHtml()
        {
            var button = htmlHelper.Button("my button");

            button.ToHtmlString().Should().Be(@"<button class=""govuk-button"">my button</button>");
        }

        [Fact]
        public void SecondaryButton_ShouldRenderCorrectHtml()
        {
            var button = htmlHelper.SecondaryButton("my button");

            button.ToHtmlString().Should().Be(@"<button class=""govuk-button govuk-button--secondary"">my button</button>");
        }
    }
}
