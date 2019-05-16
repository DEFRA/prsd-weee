namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ObligatedSentOnValuesCopyPasteControllerTests
    {
        private readonly ObligatedSentOnValuesCopyPasteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IWeeeClient weeeClient;

        public ObligatedSentOnValuesCopyPasteControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            weeeClient = weeeClient = A.Fake<IWeeeClient>();

            controller = new ObligatedSentOnValuesCopyPasteController(() => weeeClient, breadcrumb, cache);
        }

        [Fact]
        public void ObligatedSentOnValuesCopyPasteControllerInheritsAatfReturnBaseController()
        {
            typeof(ObligatedValuesCopyPasteController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ObligatedSentOnValuesCopyPasteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ObligatedValuesCopyPasteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_ApiShouldBeCalled()
        {
            Guid returnId = Guid.NewGuid();

            await controller.Index(returnId, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ViewModelShouldBeBuilt()
        {
            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();
            string siteName = "site name";
            ReturnData returnData = A.Fake<ReturnData>();

            A.CallTo(() => returnData.ReturnOperatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(returnData);

            ViewResult result = await controller.Index(returnId, organisationId, weeeSentOnId, aatfId, siteName) as ViewResult;

            ObligatedSentOnValuesCopyPasteViewModel viewModel = result.Model as ObligatedSentOnValuesCopyPasteViewModel;
            viewModel.AatfId.Should().Be(aatfId);
            viewModel.OrganisationId.Should().Be(organisationId);
            viewModel.ReturnId.Should().Be(returnId);
            viewModel.SiteName.Should().Be(siteName);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            Guid organisationId = Guid.NewGuid();
            ReturnData returnData = A.Fake<ReturnData>();
            SchemePublicInfo schemeInfo = A.Fake<SchemePublicInfo>();
            OperatorData operatorData = A.Fake<OperatorData>();
            const string orgName = "orgName";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);
            A.CallTo(() => operatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => returnData.ReturnOperatorData).Returns(operatorData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToObligatedReceived()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid schemeId = Guid.NewGuid();
            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel();
            viewModel.SiteName = siteName;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;
            viewModel.B2bPastedValues = new string[1];
            viewModel.B2cPastedValues = new string[1];

            httpContext.RouteData.Values.Add("returnId", returnId);
            httpContext.RouteData.Values.Add("aatfId", aatfId);

            RedirectToRouteResult result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ObligatedSentOn");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithBothPastedValues_TempDataShouldBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string[] pastedValues = new string[1] { "2\n" };
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel();
            viewModel.SiteName = siteName;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;
            viewModel.B2bPastedValues = pastedValues;
            viewModel.B2cPastedValues = pastedValues;

            RedirectToRouteResult result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().NotBeNull();
        }

        [Fact]
        public async void IndexPost_OnSubmitWithOnePastedValues_TempDataShouldBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel();
            viewModel.SiteName = siteName;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;
            viewModel.B2bPastedValues = new string[1] { "2\n" };
            viewModel.B2cPastedValues = new string[1];

            RedirectToRouteResult result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().NotBeNull();
        }

        [Fact]
        public async void IndexPost_OnSubmitWithNoPastedValues_TempDataShouldNotBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel();
            viewModel.SiteName = siteName;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;
            viewModel.B2bPastedValues = new string[1];
            viewModel.B2cPastedValues = new string[1];

            RedirectToRouteResult result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().BeNull();
        }

        [Fact]
        public async void IndexPost_OnCancelWithBothPastedValues_TempDataShouldNotBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string[] pastedValues = new string[1] { "2\n" };
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel();
            viewModel.SiteName = siteName;
            viewModel.ReturnId = returnId;
            viewModel.AatfId = aatfId;
            viewModel.B2bPastedValues = pastedValues;
            viewModel.B2cPastedValues = pastedValues;

            RedirectToRouteResult result = await controller.Index(viewModel, "cancel") as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().BeNull();
        }
    }
}
