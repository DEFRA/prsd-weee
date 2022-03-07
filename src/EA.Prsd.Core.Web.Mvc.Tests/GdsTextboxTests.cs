namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using EA.Prsd.Core.Web.Mvc.RazorHelpers;
    using EA.Prsd.Core.Web.Mvc.Tests.Helpers;
    using EA.Prsd.Core.Web.Mvc.Tests.ViewModels;
    using FluentAssertions;
    using System.Web.Mvc;
    using Xunit;

    public class GdsTextboxTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();

        [Fact]
        public void GivenGDSTextBox_ShouldContainGdsCssClass()
        {
            var control = htmlHelper.Gds().TextBoxFor(m => m.Nested.Bottom);

            control.ToString().Should().Contain("class").And.Contain("govuk-input");
        }
    }
}
