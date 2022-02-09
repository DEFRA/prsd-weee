﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using Core.DataReturns;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class CheckYourReturnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly CheckYourReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public CheckYourReturnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new CheckYourReturnController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckCheckYourReturnControllerInheritsExternalSiteController()
        {
            typeof(CheckYourReturnController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void CheckYourReturnController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(CheckYourReturnController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());

            await controller.Index(returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckYourReturnViewModelShouldBeBuilt()
        {
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnViewModel>(@return)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());

            await controller.Index(returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckYourReturnViewModelShouldBeReturned()
        {
            var model = A.Fake<ReturnViewModel>();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());
            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [Fact]
        public async void IndexPost_GivenReturn_SubmitReturnRequestShouldBeMade()
        {
            var model = new ReturnViewModel(new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() });

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitReturn>.That.Matches(c => c.ReturnId.Equals(model.ReturnId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexPost_GivenSubmittedReturn_ShouldRedirectToSubmittedReturnScreen()
        {
            var model = new ReturnViewModel(new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() });

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.Default);
            result.RouteValues["controller"].Should().Be("SubmittedReturn");
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
        }
    }
}
