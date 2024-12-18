namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using AutoFixture;
    using Core.Admin;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Weee.Requests.AatfReturn.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnsSummaryControllerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeClient weeeClient;
        private readonly ReturnsSummaryController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public ReturnsSummaryControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new ReturnsSummaryController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckReturnsSummaryControllerInheritsExternalSiteController()
        {
            typeof(ReturnsSummaryController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public async Task IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async Task IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenReturn_ReturnsSummaryViewModelShouldBeBuilt()
        {
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnViewModel>(@return)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async Task IndexGet_GivenReturn_ReturnsSummaryViewModelShouldBeReturned()
        {
            var model = A.Fake<ReturnViewModel>();
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenReturnWithoutSubmittedDate_ShouldBeRedirectedToSummaryScreen()
        {
            var model = A.Fake<ReturnViewModel>();
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(DateTime.Now.Year, QuarterType.Q1))
                .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                .Without(r => r.SubmittedDate)
                .Create();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>());

            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;

            redirectResult.RouteValues["action"].Should().Be("Index");
            redirectResult.RouteValues["controller"].Should().Be("Returns");
            redirectResult.RouteValues["organisationId"].Should().Be(@return.OrganisationData.Id);
        }

        [Fact]
        public async Task DownloadGet_GivenReturnIdAndObligatedRequest_RequestShouldBeSent()
        {
            var returnId = fixture.Create<Guid>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).Returns(fixture.Create<CSVFileData>());

            await controller.Download(returnId, true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadGet_GivenReturnIdAndObligatedRequest_FileContentShouldBeReturned()
        {
            var returnId = fixture.Create<Guid>();
            var file = fixture.Create<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).Returns(file);

            var result = await controller.Download(returnId, true) as FileContentResult;

            result.FileDownloadName.Should().Be(file.FileName);
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(file.FileContent));
        }

        [Fact]
        public async Task DownloadGet_GivenReturnIdAndNonObligatedRequest_RequestShouldBeSent()
        {
            var returnId = fixture.Create<Guid>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnNonObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).Returns(fixture.Create<CSVFileData>());

            await controller.Download(returnId, false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnNonObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadGet_GivenReturnIdAndNonObligatedRequest_FileContentShouldBeReturned()
        {
            var returnId = fixture.Create<Guid>();
            var file = fixture.Create<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnNonObligatedCsv>.That.Matches(r => r.ReturnId == returnId))).Returns(file);

            var result = await controller.Download(returnId, false) as FileContentResult;

            result.FileDownloadName.Should().Be(file.FileName);
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(file.FileContent));
        }
    }
}
