namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using System;
    using Xunit;

    public class GetSentOnAatfSiteRequestCreatorTests
    {
        private readonly IGetSentOnAatfSiteRequestCreator getRequestCreator;

        public GetSentOnAatfSiteRequestCreatorTests()
        {
            getRequestCreator = new GetSentOnAatfSiteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new SentOnCreateSiteOperatorViewModel()
            {
                WeeeSentOnId = Guid.NewGuid()
            };

            var request = getRequestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldBeMapped()
        {
            var weeeSentOnId = Guid.NewGuid();

            var viewModel = new SentOnCreateSiteOperatorViewModel()
            {
                WeeeSentOnId = weeeSentOnId
            };

            var request = getRequestCreator.ViewModelToRequest(viewModel);

            request.WeeeSentOnId.Should().Be(weeeSentOnId);
        }
    }
}
