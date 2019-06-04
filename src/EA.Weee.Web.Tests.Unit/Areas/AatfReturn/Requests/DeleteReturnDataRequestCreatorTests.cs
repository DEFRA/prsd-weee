namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class DeleteReturnDataRequestCreatorTests
    {
        private readonly IDeleteReturnDataRequestCreator requestCreator;

        public DeleteReturnDataRequestCreatorTests()
        {
            requestCreator = new DeleteReturnDataRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new SelectReportOptionsNilViewModel() { ReturnId = Guid.NewGuid() };

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_DeselectedOptionsMapped()
        {
            var viewModel = new SelectReportOptionsNilViewModel() { ReturnId = Guid.NewGuid() };
            var reportOptions = Enum.GetValues(typeof(ReportOnQuestionEnum));

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.DeselectedOptions.Count.Should().Be(reportOptions.Length);
            foreach (var option in reportOptions)
            {
                request.DeselectedOptions.Should().Contain((int)option);
            }
        }
    }
}
