namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Validation;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class SelectYourPCSViewModelTests
    {
        private readonly SelectYourPCSViewModel model;

        public SelectYourPCSViewModelTests()
        {
            model = new SelectYourPCSViewModel();
        }

        [Fact]
        public void SelectYourPCSViewModel_GivenSchemeListIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var selectYourPCSModel = new SelectYourPCSViewModel(null, A.Fake<List<Guid>>());
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SelectYourPCSViewModel_GivenSelectedSchemesListIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var selectYourPCSModel = new SelectYourPCSViewModel(A.Fake<List<SchemeData>>(), null);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SelectYourPCSViewModel_SelectedSchemesVariableShouldHaveMinimumElementsAttribute()
        {
            var t = typeof(SelectYourPCSViewModel);
            var pi = t.GetProperty("SelectedSchemes");
            var hasAttribute = Attribute.IsDefined(pi, typeof(MinimumElementsAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
