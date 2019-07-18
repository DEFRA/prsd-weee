namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
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
        private readonly IDeleteReturnDataRequestCreator requestCreator;

        public SelectReportOptionsNilControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMap<ReturnDataToSelectReportOptionsNilViewModelMapTransfer, SelectReportOptionsNilViewModel>>();
            requestCreator = A.Fake<IDeleteReturnDataRequestCreator>();

            controller = new SelectReportOptionsNilController(() => weeeClient, breadcrumb, cache, mapper, requestCreator);
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
            var @return = A.Fake<ReturnData>();

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30), (int)Core.DataReturns.QuarterType.Q1);
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_MapperIsCalled()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var returnData = new ReturnData()
            {
                QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), (int)Core.DataReturns.QuarterType.Q1),
                Quarter = new Quarter(2019, QuarterType.Q1)
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId == returnId))).Returns(returnData);
            
            await controller.Index(organisationId, returnId);

            A.CallTo(() => mapper.Map(A<ReturnDataToSelectReportOptionsNilViewModelMapTransfer>.That.Matches(t => t.OrganisationId == organisationId
                                                                                                               && t.ReturnId == returnId
                                                                                                               && t.ReturnData == returnData))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenReturn_SubmitReturnRequestShouldBeMade()
        {
            var model = new SelectReportOptionsNilViewModel() { ReturnId = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitReturn>.That.Matches(c => c.ReturnId.Equals(model.ReturnId) && c.NilReturn.Equals(true))))
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
