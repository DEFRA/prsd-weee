namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using EA.Prsd.Core.Web.Mvc.RazorHelpers;
    using EA.Prsd.Core.Web.Mvc.Tests.Helpers;
    using EA.Prsd.Core.Web.Mvc.Tests.ViewModels;
    using FluentAssertions;
    using System.Web.Mvc;
    using Xunit;

    public class GdsPassswordTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();

        [Fact]
        public void GivenGDSPassword_ShouldContainGdsCssClass()
        {
            var control = htmlHelper.Gds().PasswordFor(m => m.Nested.Bottom);

            control.ToString().Should().Contain("class").And.Contain("govuk-input");
        }

        [Fact]
        public void GivenGDSPassword_ShouldContainAutocompleteFalseAttribute()
        {
            var control = htmlHelper.Gds().PasswordFor(m => m.Nested.Bottom);

            control.ToString().Should().Contain("autocomplete=\"off\"");
        }
    }
}
