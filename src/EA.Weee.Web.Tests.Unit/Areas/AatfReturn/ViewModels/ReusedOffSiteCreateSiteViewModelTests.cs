namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ReusedOffSiteCreateSiteViewModelTests
    {
        [Fact]
        public void Edit_GivenSiteIdHasValue_TrueShouldBeReturned()
        {
            var model = new ReusedOffSiteCreateSiteViewModel() { SiteId = Guid.NewGuid() };

            model.Edit.Should().BeTrue();
        }

        [Fact]
        public void Edit_GivenSiteIdHasNoValue_FalseShouldBeReturned()
        {
            var model = new ReusedOffSiteCreateSiteViewModel() { SiteId = null };

            model.Edit.Should().BeFalse();
        }
    }
}
