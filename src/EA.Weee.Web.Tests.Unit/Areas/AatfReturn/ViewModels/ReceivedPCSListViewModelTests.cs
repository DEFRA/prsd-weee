namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System.Linq;
    using Core.DataReturns;
    using FluentAssertions;
    using Xunit;
    public class ReceivedPCSListViewModelTests
    {
        private readonly ReceivedPCSListViewModel model;
        public ReceivedPCSListViewModelTests()
        {
            model = new ReceivedPCSListViewModel();
        }

        [Theory]
        [InlineData("Test Organisation")]
        public void GivenReceivedPCSListViewModelDataOrganisationNameShouldBePopulated(string organisationName)
        {
            var result = model.OrganisationName = organisationName;
            model.OrganisationName.Should().Be("Test Organisation"); 
        }
    }
}
