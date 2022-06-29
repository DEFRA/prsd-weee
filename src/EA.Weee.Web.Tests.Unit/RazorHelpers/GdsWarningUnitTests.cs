namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Web.RazorHelpers;
    using Xunit;

    public class GdsWarningUnitTests
    {
        private readonly WeeeGds<TestModel> htmlHelper;

        public GdsWarningUnitTests()
        {
            htmlHelper = new WeeeGds<TestModel>(A.Fake<WebViewPage<TestModel>>());
        }

        [Fact]
        public void GivenGDSPanel_HtmlShouldBeValid()
        {
            var control = htmlHelper.Warning("display text");

            control.ToString().Should().Be(@"<div class=""govuk-warning-text""><span aria-hidden=""true"" class=""govuk-warning-text__icon"">!</span><strong class=""govuk-warning-text__text""><span class=""govuk-warning-text__assistive"">Warning</span>display text</strong></div>");
        }
    }
}
