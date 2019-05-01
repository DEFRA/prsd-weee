namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddSelectReportOptionsRequestCreatorTests
    {
        private readonly AddSelectReportOptionsRequestCreator requestCreator;

        public AddSelectReportOptionsRequestCreatorTests()
        {
            requestCreator = new AddSelectReportOptionsRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new SelectReportOptionsViewModel()
            {
                ReturnId = A.Dummy<Guid>(),
                SelectedOptions = A.Fake<List<int>>(),
                DcfSelectedValue = A.Dummy<string>(),
                ReportOnQuestions = A.Fake<List<ReportOnQuestion>>()
            };
            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }
    }
}
