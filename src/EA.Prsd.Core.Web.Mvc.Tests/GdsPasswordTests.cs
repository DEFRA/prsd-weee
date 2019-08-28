using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace EA.Prsd.Core.Web.Mvc.Tests
{
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Helpers;
    using RazorHelpers;
    using ViewModels;
    using FakeItEasy;
    using FluentAssertions;

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
