﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
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
    using System;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class ObligatedValuesCopyPasteControllerTests
    {
        private readonly ObligatedValuesCopyPasteController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IWeeeClient weeeClient;

        public ObligatedValuesCopyPasteControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            weeeClient = weeeClient = A.Fake<IWeeeClient>();

            controller = new ObligatedValuesCopyPasteController(() => weeeClient, breadcrumb, cache);
        }

        [Fact]
        public void ObligatedValuesCopyPasteControllerInheritsAatfReturnBaseController()
        {
            typeof(ObligatedValuesCopyPasteController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ObligatedValuesCopyPasteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ObligatedValuesCopyPasteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_ApiShouldBeCalled()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<ObligatedType>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ViewModelShouldBeBuilt()
        {
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfName = "Test Aatf";
            var aatfData = A.Fake<AatfData>();
            var schemeId = Guid.NewGuid();
            var schemeName = "Test Scheme";
            var schemeInfo = A.Fake<SchemePublicInfo>();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.OrganisationData.Id).Returns(organisationId);
            A.CallTo(() => schemeInfo.Name).Returns(schemeName);
            A.CallTo(() => cache.FetchSchemePublicInfoBySchemeId(schemeId)).Returns(schemeInfo);
            A.CallTo(() => aatfData.Name).Returns(aatfName);
            A.CallTo(() => cache.FetchAatfData(@return.OrganisationData.Id, aatfId)).Returns(aatfData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            var result = await controller.Index(returnId, aatfId, schemeId, ObligatedType.Received) as ViewResult;

            var viewModel = result.Model as ObligatedValuesCopyPasteViewModel;
            viewModel.AatfId.Should().Be(aatfId);
            viewModel.AatfName.Should().Be(aatfName);
            viewModel.OrganisationId.Should().Be(organisationId);
            viewModel.ReturnId.Should().Be(returnId);
            viewModel.SchemeId.Should().Be(schemeId);
            viewModel.SchemeName.Should().Be(schemeName);
        }

        [Fact]
        public async void IndexGet_FromReuse_GivenReturn_ViewModelShouldBeBuilt()
        {
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfName = "Test Aatf";
            var aatfData = A.Fake<AatfData>();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.OrganisationData.Id).Returns(organisationId);
            A.CallTo(() => aatfData.Name).Returns(aatfName);
            A.CallTo(() => cache.FetchAatfData(@return.OrganisationData.Id, aatfId)).Returns(aatfData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            var result = await controller.Index(returnId, aatfId, Guid.Empty, ObligatedType.Reused) as ViewResult;

            var viewModel = result.Model as ObligatedValuesCopyPasteViewModel;
            viewModel.AatfId.Should().Be(aatfId);
            viewModel.AatfName.Should().Be(aatfName);
            viewModel.OrganisationId.Should().Be(organisationId);
            viewModel.ReturnId.Should().Be(returnId);
            viewModel.SchemeId.Should().Be(Guid.Empty);
            viewModel.SchemeName.Should().Be(null);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => organisationData.Id).Returns(organisationId);
            A.CallTo(() => @return.OrganisationData).Returns(organisationData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(A.Dummy<Guid>(), aatfId, A.Dummy<Guid>(), A.Dummy<ObligatedType>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexPost_FromReceived_OnSubmit_PageRedirectsToObligatedReceived()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid schemeId = Guid.NewGuid();
            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();

            ObligatedValuesCopyPasteViewModel viewModel = new ObligatedValuesCopyPasteViewModel
            {
                SchemeId = schemeId,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new String[1],
                B2cPastedValues = new String[1],
                Type = ObligatedType.Received
            };

            httpContext.RouteData.Values.Add("schemeId", schemeId);
            httpContext.RouteData.Values.Add("returnId", returnId);
            httpContext.RouteData.Values.Add("aatfId", aatfId);

            RedirectToRouteResult result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ObligatedReceived");
            result.RouteValues["schemeId"].Should().Be(schemeId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_FromReused_OnSubmit_PageRedirectsToObligatedReused()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();

            ObligatedValuesCopyPasteViewModel viewModel = new ObligatedValuesCopyPasteViewModel
            {
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new String[1],
                B2cPastedValues = new String[1],
                Type = ObligatedType.Reused
            };

            httpContext.RouteData.Values.Add("returnId", returnId);
            httpContext.RouteData.Values.Add("aatfId", aatfId);

            RedirectToRouteResult result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ObligatedReused");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async void IndexPost_OnSubmitWithBothPastedValues_TempDataShouldBeAttached()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var pastedValues = new String[1] { "2\n" };

            var viewModel = new ObligatedValuesCopyPasteViewModel
            {
                SchemeId = schemeId,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = pastedValues,
                B2cPastedValues = pastedValues
            };

            await controller.Index(viewModel);

            controller.TempData["pastedValues"].Should().NotBeNull();
        }

        [Fact]
        public async void IndexPost_OnSubmitWithOnePastedValues_TempDataShouldBeAttached()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var schemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var viewModel = new ObligatedValuesCopyPasteViewModel
            {
                SchemeId = schemeId,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new String[1] { "2\n" },
                B2cPastedValues = new String[1]
            };

            await controller.Index(viewModel);

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

            var viewModel = new ObligatedValuesCopyPasteViewModel
            {
                SchemeId = schemeId,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new String[1],
                B2cPastedValues = new String[1]
            };

            await controller.Index(viewModel);

            controller.TempData["pastedValues"].Should().BeNull();
        }
    }
}
