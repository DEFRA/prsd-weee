namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetWeeeSentOnRequestCreatorTests
    {
        private readonly IGetWeeeSentOnRequestCreator requestCreator;

        public GetWeeeSentOnRequestCreatorTests()
        {
            this.requestCreator = new GetWeeeSentOnRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new SentOnSiteSummaryListViewModel()
            {
                AatfId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid()
            };

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldBeMapped()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var viewModel = new SentOnSiteSummaryListViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId
            };

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.ReturnId.Should().Be(returnId);
            request.AatfId.Should().Be(aatfId);
        }
    }
}