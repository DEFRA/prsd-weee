namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SelectReportOptionsNilControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel> mapper;
        private readonly SelectReportOptionsNilController controller;

        public SelectReportOptionsNilControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel>>();

            controller = new SelectReportOptionsNilController(() => weeeClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void SelectReportOptionsNilController_InheritsBaseController()
        {
            typeof(SelectReportOptionsNilController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectReportOptionsNilController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectReportOptionsNilController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public void SelectReportOptionsNilController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SelectReportOptionsNilController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
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
        public async void IndexPost_GivenReturn_SubmitReturnRequestShouldBeMade()
        {
            var model = new SelectReportOptionsNilViewModel() { ReturnId = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitReturn>.That.Matches(c => c.ReturnId.Equals(model.ReturnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenSubmittedReturn_ShouldRedirectToSubmittedReturnScreen()
        {
            var model = new SelectReportOptionsNilViewModel() { ReturnId = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.Default);
            result.RouteValues["controller"].Should().Be("SubmittedReturn");
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
        }
    }
}
