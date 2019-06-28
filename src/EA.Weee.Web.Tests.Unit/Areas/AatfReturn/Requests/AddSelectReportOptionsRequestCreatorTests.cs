namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull(bool dcfSelected)
        {
            var viewModel = A.Fake<SelectReportOptionsViewModel>();

            A.CallTo(() => viewModel.ReturnId).Returns(Guid.NewGuid());
            A.CallTo(() => viewModel.SelectedOptions).Returns(new List<int>() { 1 });
            A.CallTo(() => viewModel.DeSelectedOptions).Returns(new List<int>() { 1, 2 });
            A.CallTo(() => viewModel.ReportOnQuestions).Returns(new List<ReportOnQuestion>());
            A.CallTo(() => viewModel.DcfQuestionSelected).Returns(dcfSelected);

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
            request.SelectedOptions.Should().BeSameAs(viewModel.SelectedOptions);
            request.DcfSelectedValue.Should().Be(dcfSelected);
            request.DeselectedOptions.Should().BeSameAs(viewModel.DeSelectedOptions);
            request.Options.Should().BeSameAs(viewModel.ReportOnQuestions);
            request.ReturnId.Should().Be(viewModel.ReturnId);
        }
    }
}
