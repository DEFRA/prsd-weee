namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.DataReturns;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class AatfTaskListControllerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeClient weeeClient;
        private readonly AatfTaskListController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public AatfTaskListControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            controller = new AatfTaskListController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void AatfTaskListControllerInheritsExternalSiteController()
        {
            typeof(AatfTaskListController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void AatfTaskListController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(AatfTaskListController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(2019, QuarterType.Q1))
                .With(r => r.QuarterWindow, new QuarterWindow(new DateTime(2019, 01, 01), new DateTime(2019, 03, 31), (int)QuarterType.Q1))
                .Create();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            await controller.Index(A.Dummy<Guid>());
            SystemTime.Unfreeze();

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }
        
        [Fact]
        public async void IndexPost_OnAnySubmit_PageRedirectsToCheckYourReturn()
        {
            var model = new ReturnViewModel(new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = new QuarterWindow(DateTime.Today, DateTime.Today, (int)QuarterType.Q1) });

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("CheckYourReturn");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
        }

        [Fact]
        public async void IndexGet_GivenReturn_AatfTaskListViewModelShouldBeBuilt()
        {
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(2019, QuarterType.Q1))
                .With(r => r.QuarterWindow, new QuarterWindow(new DateTime(DateTime.Now.Year, 01, 01), new DateTime(DateTime.Now.Year, 03, 31), (int)QuarterType.Q1))
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            await controller.Index(A.Dummy<Guid>());
            SystemTime.Unfreeze();

            A.CallTo(() => mapper.Map<ReturnViewModel>(@return)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_AatfTaskListViewModelShouldBeReturned()
        {
            var model = A.Fake<ReturnViewModel>();
            var @return = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(2019, QuarterType.Q1))
                .With(r => r.QuarterWindow, new QuarterWindow(new DateTime(DateTime.Now.Year, 01, 01), new DateTime(DateTime.Now.Year, 03, 31), (int)QuarterType.Q1))
                .Create();

            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).Returns(model);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;
            SystemTime.Unfreeze();

            result.Model.Should().BeEquivalentTo(model);
        }
    }
}
