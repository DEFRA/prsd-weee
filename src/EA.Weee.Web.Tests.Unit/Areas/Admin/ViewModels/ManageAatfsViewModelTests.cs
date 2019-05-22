namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using Xunit;

    public class ManageAatfsViewModelTests
    {
        [Fact]
        public void ManageAatfsViewModel_RequiredVariableShouldHaveRequiredAttribute()
        {
            var t = typeof(ManageAatfsViewModel);
            var pi = t.GetProperty("Selected");
            var hasAttribute = Attribute.IsDefined(pi, typeof(RequiredAttribute));

            hasAttribute.Should().Be(true);
        }
    }
}
