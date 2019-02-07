namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using TestHelpers;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.Requests;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.AatfReturn.NonObligated;
    using Xunit;

    public class SubmittedReturnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SubmittedReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public SubmittedReturnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new SubmittedReturnController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckSubmittedReturnControllerInheritsExternalSiteController()
        {
            typeof(SubmittedReturnController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(A.Dummy<Guid>(), returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckReturnViewModelShouldBeBuilt()
        {
            var returnData = new ReturnData();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(returnData)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckReturnViewModelShouldBeReturned()
        {
            var model = A.Fake<SubmittedReturnViewModel>();

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexPost_GivenModel_RedirectShouldBeCorrect()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            httpContext.RouteData.Values.Add("organisationId", 1);

            var redirect = await controller.Index(A.Dummy<SubmittedReturnViewModel>()) as RedirectToRouteResult;

            redirect.RouteValues["action"].Should().Be("ChooseActivity");
            redirect.RouteValues["controller"].Should().Be("Home");
            redirect.RouteValues["area"].Should().Be("Scheme");
            redirect.RouteValues["pcsId"].Should().Be(1);
        }
    }
}
