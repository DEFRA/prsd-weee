namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
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

            button.ToHtmlString().Should().Be(@"<button class=""govuk-button"" data-module=""govuk-button"" data-prevent-double-click=""true"" type=""submit"">my button</button>");
        }

        [Fact]
        public void SecondaryButton_ShouldRenderCorrectHtml()
        {
            var button = htmlHelper.Button("my button", secondaryButton: true);

            button.ToHtmlString().Should().Be(@"<button class=""govuk-button govuk-button--secondary"" data-module=""govuk-button"" data-prevent-double-click=""true"" type=""submit"">my button</button>");
        }
    }
}
