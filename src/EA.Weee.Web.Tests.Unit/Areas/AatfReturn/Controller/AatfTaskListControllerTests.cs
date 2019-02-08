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
    using Xunit;

    public class AatfTaskListControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly AatfTaskListController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public AatfTaskListControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            controller = new AatfTaskListController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void CheckAatfTaskListControllerInheritsExternalSiteController()
        {
            typeof(NonObligatedController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }
        
        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }
        
        [Fact]
        public async void IndexPost_OnAnySubmit_PageRedirectsToCheckYourReturn()
        {
            var model = new ReturnViewModel();

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("CheckYourReturn");
            result.RouteValues["area"].Should().Be("AatfReturn");
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenReturn_AatfTaskListViewModelShouldBeBuilt()
        {
            var returnData = new ReturnData();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnViewModel>(returnData)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_AatfTaskListViewModelShouldBeReturned()
        {
            var model = A.Fake<ReturnViewModel>();

            A.CallTo(() => mapper.Map<ReturnViewModel>(A< ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }
    }
}
