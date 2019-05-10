namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Infrastructure;
    using Xunit;

    public class ReturnsSummaryControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly CheckYourReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public ReturnsSummaryControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new CheckYourReturnController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckReturnsSummaryControllerInheritsExternalSiteController()
        {
            typeof(ReturnsSummaryController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());

            await controller.Index(returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ReturnsSummaryViewModelShouldBeBuilt()
        {
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnViewModel>(@return)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());

            await controller.Index(returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ReturnsSummaryViewModelShouldBeReturned()
        {
            var model = A.Fake<ReturnViewModel>();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());
            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }
    }
}
