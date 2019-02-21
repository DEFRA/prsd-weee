namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System.Linq;
    using Core.DataReturns;
    using FluentAssertions;
    using Xunit;
    public class ReceivedPcsListViewModelTests
    {
        private readonly ReceivedPcsListViewModel model;
        public ReceivedPcsListViewModelTests()
        {
            model = new ReceivedPcsListViewModel();
        }

        [Theory]
        [InlineData("Test Organisation")]
        public void GivenReceivedPCSListViewModelDataOrganisationNameShouldBePopulated(string organisationName)
        {
            var result = model.AatfName = organisationName;
            model.AatfName.Should().Be("Test Organisation"); 
        }
    }
}
