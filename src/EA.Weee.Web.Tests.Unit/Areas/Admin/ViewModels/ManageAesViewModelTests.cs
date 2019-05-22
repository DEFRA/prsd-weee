namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Admin.ViewModels.Ae;
    using FluentAssertions;
    using Xunit;

    public class ManageAesViewModelTests
    {
        [Fact]
        public void ManageAesViewModel_RequiredVariableShouldHaveRequiredAttribute()
        {
            var t = typeof(ManageAesViewModel);
            var pi = t.GetProperty("Selected");
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
