namespace EA.Weee.Web.Tests.Unit.RazorHelpers
{
    using System.Web.Mvc;
    using FluentAssertions;
    using Prsd.Core.Web.Mvc.RazorHelpers;
    using Prsd.Core.Web.Mvc.Tests.Helpers;
    using Prsd.Core.Web.Mvc.Tests.ViewModels;
    using Xunit;

    public class GdsLabelUnitTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();

        [Fact]
        public void GivenGDSLabel_ShouldHaveId()
        {
            var control = htmlHelper.Gds().LabelFor(m => m.TopLevel);

            control.ToString().Should().Contain("id=\"TopLevel-label\"");
        }
    }
}
