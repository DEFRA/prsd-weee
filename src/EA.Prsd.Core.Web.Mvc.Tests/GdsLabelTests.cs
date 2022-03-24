namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using System.Web.Mvc;
    using FluentAssertions;
    using Helpers;
    using RazorHelpers;
    using ViewModels;
    using Xunit;

    public class GdsLabelTests
    {
        private readonly HtmlHelper<TestModel> htmlHelper = HtmlHelperFactory.CreateHtmlHelper<TestModel>();
        private const string OptionalText = "optional";

        [Fact]
        public void GivenGDLabel_WithNoHtmlAttributes_AndNonRequiredField_AndNoOptionalLabel_ShouldNotContainOptionalText()
        {
            var control = htmlHelper.Gds().LabelFor(m => m.TopLevel, false);

            control.ToString().Should().NotContain(OptionalText);
        }

        [Fact]
        public void GivenGDLabel_WithHtmlAttributes_AndNonRequiredField_AndNoOptionalLabel_ShouldNotContainOptionalText()
        {
            var control = htmlHelper.Gds().LabelFor(m => m.TopLevel, new {}, false);

            control.ToString().Should().NotContain(OptionalText);
        }

        [Fact]
        public void GivenGDLabel_WithNonRequiredField_AndOptionalLabel_ShouldContainOptionalText()
        {
            var control = htmlHelper.Gds().LabelFor(m => m.TopLevel, true);

            control.ToString().Should().Contain(OptionalText);
        }

        [Fact]
        public void GivenGDLabel_WithHtmlAttributes_AndNoWithRequiredField_AndOptionalLabel_ShouldContainOptionalText()
        {
            var control = htmlHelper.Gds().LabelFor(m => m.TopLevel, new {}, true);

            control.ToString().Should().Contain(OptionalText);
        }
    }
}
