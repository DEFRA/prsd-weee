namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
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

    public class NonObligatedValuesCopyPasteControllerTests
    {
        private readonly NonObligatedValuesCopyPasteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IWeeeClient weeeClient;

        public NonObligatedValuesCopyPasteControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            weeeClient = weeeClient = A.Fake<IWeeeClient>();

            controller = new NonObligatedValuesCopyPasteController(() => weeeClient, breadcrumb, cache);
        }

        [Fact]
        public void NonObligatedValuesCopyPasteControllerInheritsAatfReturnBaseController()
        {
            typeof(ObligatedValuesCopyPasteController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void NonObligatedValuesCopyPasteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ObligatedValuesCopyPasteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_ApiShouldBeCalled()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId, false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ViewModelShouldBeBuilt()
        {
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.OrganisationData.Id).Returns(organisationId);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            var result = await controller.Index(returnId, A.Dummy<bool>()) as ViewResult;

            var viewModel = result.Model as NonObligatedValuesCopyPasteViewModel;
            viewModel.OrganisationId.Should().Be(organisationId);
            viewModel.ReturnId.Should().Be(returnId);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => organisationData.Id).Returns(organisationId);
            A.CallTo(() => @return.OrganisationData).Returns(organisationData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<bool>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToNonObligated()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var viewModel = new NonObligatedValuesCopyPasteViewModel();
            viewModel.ReturnId = returnId;
            viewModel.PastedValues = new String[1];
            viewModel.Dcf = false;

            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            result.RouteName.Should().Be("aatf-non-obligated");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["dcf"].Should().Be(false);
        }

        [Fact]
        public async void SetsHeaderTitle_For_NonObligatedCopyPaste()
        {
            var typeheading = "Non-obligated WEEE";

            var result = await controller.Index(A.Dummy<Guid>(), false) as ViewResult;
            var viewModel = result.Model as NonObligatedValuesCopyPasteViewModel;
            viewModel.Typeheading.Should().Be(typeheading);
        }

        [Fact]
        public async void SetsHeaderTitle_For_DcfObligatedCopyPaste()
        {
            var typeheading = "Non-obligated WEEE retained by a DCF";

            var result = await controller.Index(A.Dummy<Guid>(), true) as ViewResult;
            var viewModel = result.Model as NonObligatedValuesCopyPasteViewModel;
            viewModel.Typeheading.Should().Be(typeheading);
        }

        [Fact]
        public async void IndexPost_OnSubmitDcf_PageRedirectsToNonObligatedDcf()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var viewModel = new NonObligatedValuesCopyPasteViewModel();
            viewModel.ReturnId = returnId;
            viewModel.PastedValues = new String[1];
            viewModel.Dcf = true;

            httpContext.RouteData.Values.Add("returnId", returnId);

            var result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            result.RouteName.Should().Be("aatf-non-obligated-dcf");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["dcf"].Should().Be(true);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithPastedValues_TempDataShouldBeAttached()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var pastedValues = new String[1] { "2\n" };

            var viewModel = new NonObligatedValuesCopyPasteViewModel();
            viewModel.ReturnId = returnId;
            viewModel.PastedValues = pastedValues;
            
            var result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().NotBeNull();
        }
        
        [Fact]
        public async void IndexPost_OnSubmitWithNoPastedValues_TempDataShouldNotBeAttached()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var viewModel = new NonObligatedValuesCopyPasteViewModel();
            viewModel.ReturnId = returnId;
            viewModel.PastedValues = new String[1];

            var result = await controller.Index(viewModel, null) as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().BeNull();
        }

        [Fact]
        public async void IndexPost_OnCancelWithPastedValues_TempDataShouldNotBeAttached()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var pastedValues = new String[1] { "2\n" };

            var viewModel = new NonObligatedValuesCopyPasteViewModel();
            viewModel.ReturnId = returnId;
            viewModel.PastedValues = pastedValues;

            var result = await controller.Index(viewModel, "cancel") as RedirectToRouteResult;

            controller.TempData["pastedValues"].Should().BeNull();
        }
    }
}
