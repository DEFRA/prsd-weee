﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using Core.DataReturns;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Web.Areas.AatfReturn.Controllers;
    using Weee.Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReturnsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public ReturnsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new ReturnsController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void ReturnsControllerInheritsAatfReturnBaseController()
        {
            typeof(ReturnsController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ReturnsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(ReturnsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_DefaultViewShouldBeReturned()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_BreadcrumbShouldBeSet()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            await controller.Index(A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsShouldBeRetrieved()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>.That.Matches(r => r.OrganisationId.Equals(organisationId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeBuilt()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnsViewModel>(returns)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeReturnedWithAllReturns()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var model = new ReturnsViewModel();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnsData>._)).Returns(model);

            var result = await controller.Index(organisationId) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;

            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
            returnedModel.Returns.Count.Should().Be(returns.ReturnsList.Count);
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public async void IndexPost_GivenOrganisationId_ReturnShouldBeCreated(QuarterType quarterType)
        {
            var model = new ReturnsViewModel() { OrganisationId = Guid.NewGuid(), ComplianceYear = 2019, Quarter = quarterType };

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturn>.That
                    .Matches(c => c.OrganisationId.Equals(model.OrganisationId)
                    && c.Quarter.Equals(model.Quarter)
                    && c.Year.Equals(model.ComplianceYear)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenOrganisationId_UserShouldBeRedirectedToOptions()
        {
            var model = new ReturnsViewModel() { OrganisationId = Guid.NewGuid() };
            var returnId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturn>._)).Returns(returnId);

            var redirectResult = await controller.Index(model) as RedirectToRouteResult;

            redirectResult.RouteName.Should().Be(AatfRedirect.SelectReportOptionsRouteName);
            redirectResult.RouteValues["action"].Should().Be("Index");
            redirectResult.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            redirectResult.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async void IndexPost_GivenFilterParameter_ViewShouldBeReturned()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var model = new ReturnsViewModel();
            var organisationId = Guid.NewGuid();
            model.OrganisationId = organisationId;

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnsData>._)).Returns(model);

            var result = await controller.Index(model, true) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;

            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
            returnedModel.Returns.Count.Should().Be(returns.ReturnsList.Count);
        }

        [Fact]
        public async void IndexPost_GivenFilterParameterAndYearFilter_ViewShouldBeReturnedWithCorrectReturns()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var model = new ReturnsViewModel();
            model.SelectedComplianceYear = 2019;
            model.SelectedQuarter = "Q1";
            var organisationId = Guid.NewGuid();
            model.OrganisationId = organisationId;

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnsData>._)).Returns(model);

            var result = await controller.Index(model, true) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;

            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
            returnedModel.Returns.Count.Should().Be(returns.ReturnsList.Count(p => p.Quarter.Q.ToString() == model.SelectedQuarter));
            returnedModel.Returns.Count.Should().Be(returns.ReturnsList.Count(p => p.Quarter.Year == model.SelectedComplianceYear));
        }

        [Fact]
        public void CopyPost_ShouldBeDecoratedWithValidateReturnEditActionFilterAttribute()
        {
            typeof(ReturnsController).GetMethod("Copy").Should().BeDecoratedWith<ValidateReturnEditActionFilterAttribute>();
        }

        [Fact]
        public void CopyPost_ShouldBeDecoratedWithRouteAttribute()
        {
            typeof(ReturnsController).GetMethod("Copy").Should().BeDecoratedWith<RouteAttribute>()
                .Which.Template.Should().Be("aatf-return/returns/{organisationId:Guid}/copy/{returnId:Guid}");
        }

        [Fact]
        public async void CopyPost_UserShouldBeRedirectedToTaskList()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<CopyReturn>.That.Matches(r => r.ReturnId == returnId)));
            clientCall.Returns(newId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            @return.NilReturn = false;

            var redirectResult = await controller.Copy(returnId, organisationId) as RedirectToRouteResult;

            redirectResult.RouteName.Should().Be(AatfRedirect.Default);
            redirectResult.RouteValues["action"].Should().Be("Index");
            redirectResult.RouteValues["returnId"].Should().Be(newId);

            clientCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void CopyPost_UserShouldBeRedirectedToReportOptionsForNilReturn()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<CopyReturn>.That.Matches(r => r.ReturnId == returnId)));
            clientCall.Returns(newId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            @return.NilReturn = true;

            var redirectResult = await controller.Copy(returnId, organisationId) as RedirectToRouteResult;

            redirectResult.RouteName.Should().Be(AatfRedirect.SelectReportOptionsRouteName);
            redirectResult.RouteValues["action"].Should().Be("Index");
            redirectResult.RouteValues["returnId"].Should().Be(newId);
            redirectResult.RouteValues["organisationId"].Should().Be(organisationId);
            clientCall.MustHaveHappenedOnceExactly();
        }
    }
}
