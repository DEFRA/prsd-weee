﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
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
        public async Task IndexGet_GivenActionExecutes_ApiShouldBeCalled()
        {
            Guid returnId = Guid.NewGuid();

            await controller.Index(returnId, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenReturn_ViewModelShouldBeBuilt()
        {
            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();
            string siteName = "site name";
            ReturnData returnData = A.Fake<ReturnData>();
            var categories = EnumHelper.GetValues(typeof(WeeeCategory));
            var maxCategoryId = categories.Max(x => x.Key);

            A.CallTo(() => returnData.OrganisationData.Id).Returns(organisationId);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(returnData);

            ViewResult result = await controller.Index(returnId, organisationId, weeeSentOnId, aatfId, siteName) as ViewResult;

            ObligatedSentOnValuesCopyPasteViewModel viewModel = result.Model as ObligatedSentOnValuesCopyPasteViewModel;
            viewModel.AatfId.Should().Be(aatfId);
            viewModel.OrganisationId.Should().Be(organisationId);
            viewModel.ReturnId.Should().Be(returnId);
            viewModel.SiteName.Should().Be(siteName);
            viewModel.WeeeCategoryCount.Should().Be(categories.Count());
            viewModel.MaxWeeeCategoryId.Should().Be(maxCategoryId);
        }

        [Fact]
        public async Task IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            Guid organisationId = Guid.NewGuid();
            ReturnData returnData = A.Fake<ReturnData>();
            OrganisationData organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            returnData.Quarter = quarterData;
            returnData.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);
            A.CallTo(() => organisationData.Id).Returns(organisationId);
            A.CallTo(() => returnData.OrganisationData).Returns(organisationData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), aatfId, A.Dummy<string>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async Task IndexPost_OnSubmit_PageRedirectsToObligatedReceived()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel
            {
                SiteName = siteName,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new string[1],
                B2cPastedValues = new string[1],
                IsEditDetails = false,
                IsEditTonnage = false
            };

            httpContext.RouteData.Values.Add("returnId", returnId);
            httpContext.RouteData.Values.Add("aatfId", aatfId);

            RedirectToRouteResult result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ObligatedSentOn");
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["aatfId"].Should().Be(aatfId);
        }

        [Fact]
        public async Task IndexPost_OnSubmitWithBothPastedValues_TempDataShouldBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string[] pastedValues = new string[1] { "2\n" };
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel
            {
                SiteName = siteName,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = pastedValues,
                B2cPastedValues = pastedValues,
                IsEditDetails = false,
                IsEditTonnage = false
            };

            await controller.Index(viewModel);

            controller.TempData["pastedValues"].Should().NotBeNull();
        }

        [Fact]
        public async Task IndexPost_OnSubmitWithOnePastedValues_TempDataShouldBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel
            {
                SiteName = siteName,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new string[1] { "2\n" },
                B2cPastedValues = new string[1],
                IsEditDetails = false,
                IsEditTonnage = false
            };

            await controller.Index(viewModel);

            controller.TempData["pastedValues"].Should().NotBeNull();
        }

        [Fact]
        public async Task IndexPost_OnSubmitWithNoPastedValues_TempDataShouldNotBeAttached()
        {
            HttpContextMocker httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            Guid returnId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            string siteName = "site name";

            ObligatedSentOnValuesCopyPasteViewModel viewModel = new ObligatedSentOnValuesCopyPasteViewModel
            {
                SiteName = siteName,
                ReturnId = returnId,
                AatfId = aatfId,
                B2bPastedValues = new string[1],
                B2cPastedValues = new string[1],
                IsEditDetails = false,
                IsEditTonnage = false
            };

            await controller.Index(viewModel);

            controller.TempData["pastedValues"].Should().BeNull();
        }
    }
}
