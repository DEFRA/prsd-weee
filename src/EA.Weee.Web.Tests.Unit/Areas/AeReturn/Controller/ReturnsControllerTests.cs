﻿namespace EA.Weee.Web.Tests.Unit.Areas.AeReturn.Controller
{
    using AutoFixture;
    using Core.DataReturns;
    using Core.Organisations;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AeReturn.Controllers;
    using EA.Weee.Web.Areas.AeReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReturnsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly Guid organisationId;
        private readonly Fixture fixture;

        public ReturnsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            organisationId = Guid.NewGuid();
            fixture = new Fixture();

            controller = new ReturnsController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void ReturnsControllerInheritsAeReturnBaseController()
        {
            typeof(ReturnsController).BaseType.Name.Should().Be(typeof(AeReturnBaseController).Name);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisation_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(fixture.Create<Guid>(), fixture.Create<int?>(), fixture.Create<string>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(ReturnsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public async Task IndexGet_GivenOrganisation_BreadcrumbShouldBeSet()
        {
            await controller.Index(fixture.Create<Guid>(), fixture.Create<int?>(), fixture.Create<string>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisation_ReturnsShouldBeRetrieved()
        {
            await controller.Index(organisationId, fixture.Create<int?>(), fixture.Create<string>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>.That.Matches(r => r.OrganisationId.Equals(organisationId))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisation_ReturnsViewModelShouldBeBuilt()
        {
            var returnsData = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returnsData);

            await controller.Index(fixture.Create<Guid>(), fixture.Create<int?>(), fixture.Create<string>());

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnToReturnsViewModelTransfer>.That.Matches(r => r.ReturnsData == returnsData))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisationComplianceYearAndQuarterActionParameters_ReturnsViewModelShouldBeBuiltWithPassedParameters()
        {
            var returns = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            var selectedComplianceYear = fixture.Create<int>();
            var selectedQuarter = fixture.Create<string>();

            await controller.Index(A.Dummy<Guid>(), selectedComplianceYear, selectedQuarter);

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnToReturnsViewModelTransfer>.That.Matches(r => r.ReturnsData == returns &&
                                                                                                              r.SelectedComplianceYear == selectedComplianceYear &&
                                                                                                              r.SelectedQuarter == selectedQuarter))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexGet_GivenOrganisation_ReturnsViewModelShouldBeReturned()
        {
            var model = new ReturnsViewModel();

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnToReturnsViewModelTransfer>._)).Returns(model);

            var result = await controller.Index(organisationId, fixture.Create<int?>(), fixture.Create<string>()) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;

            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task PostIndex_ReturnCreated_RedirectWithReturnIdShouldHappen()
        {
            var returnId = Guid.NewGuid();
            var quarter = QuarterType.Q1;
            const int year = 2019;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturn>.That.Matches(a => a.FacilityType.Equals(FacilityType.Ae)
            && a.OrganisationId.Equals(organisationId)
            && a.Quarter.Equals(quarter)
            && a.Year.Equals(year)))).Returns(returnId);

            ReturnsViewModel viewModel = A.Dummy<ReturnsViewModel>();

            viewModel.OrganisationId = organisationId;
            viewModel.Quarter = quarter;
            viewModel.ComplianceYear = year;

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ExportedWholeWeee");
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
        }

        [Fact]
        public async Task GetExportedWholeWeee_GivenOrganisation_DefaultViewShouldBeReturned()
        {
            var result = await controller.ExportedWholeWeee(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task GetExportedWholeWeee_BreadCrumbShouldBeSet()
        {
            await controller.ExportedWholeWeee(A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Theory]
        [InlineData(YesNoEnum.No, "Index")]
        [InlineData(YesNoEnum.Yes, "NilReturn")]
        public async Task PostExportedWholeWeee_SelectedValueGiven_CorrectRedirectHappens(YesNoEnum selectedValue, string action)
        {
            var viewModel = new ExportedWholeWeeeViewModel()
            {
                WeeeSelectedValue = selectedValue,
                ReturnId = Guid.NewGuid()
            };

            var result = await controller.ExportedWholeWeee(organisationId, viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);

            if (selectedValue == YesNoEnum.No)
            {
                result.RouteValues["organisationId"].Should().Be(organisationId);
            }
            if (selectedValue == YesNoEnum.Yes)
            {
                result.RouteValues["returnId"].Should().Be(viewModel.ReturnId);
            }
        }

        [Fact]
        public async Task IndexPost_RedirectToExportedWholeWeee()
        {
            var organisationId = Guid.NewGuid();

            var viewModel = new ReturnsViewModel()
            {
                OrganisationId = organisationId
            };

            var result = await controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ExportedWholeWeee");
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
        }

        [Fact]
        public async Task PostExportedWholeWeee_NoValueSelected_ReturnsViewWithViewModelAndBreadCrumbSet()
        {
            var viewModel = new ExportedWholeWeeeViewModel();

            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.ExportedWholeWeee(organisationId, viewModel) as ViewResult;

            result.ViewName.Should().BeEmpty();
            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async Task GetNilResult_GivenReturnId_BreadCrumbSet()
        {
            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData() { Id = organisationId }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            var result = await controller.NilReturn(returnData.Id) as ViewResult;

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async Task GetNilResult_GivenReturnId_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await controller.NilReturn(returnId) as ViewResult;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetNilResult_GivenReturnId_ViewModelShouldBeMapped()
        {
            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData() { Id = organisationId }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            var result = await controller.NilReturn(A.Dummy<Guid>()) as ViewResult;

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(returnData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetNilResult_GivenReturnId_MappedViewModelShouldBeReturnedToView()
        {
            var viewModel = new SubmittedReturnViewModel();

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(A<ReturnData>._)).Returns(viewModel);

            var result = await controller.NilReturn(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(viewModel);
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task PostNilResult_GivenReturnId_RedirectsToConfirmationScreen()
        {
            var model = new SubmittedReturnViewModel(new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() });

            var result = await controller.NilReturnConfirm(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Confirmation");
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
        }

        [Fact]
        public async Task PostNilResult_SubmitReturnShouldBeCalled()
        {
            var model = new SubmittedReturnViewModel(new ReturnData() { Id = Guid.NewGuid(), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() });

            await controller.NilReturnConfirm(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitReturn>.That.Matches(c => c.ReturnId.Equals(model.ReturnId) && c.NilReturn.Equals(true))))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task GetConfirmation__GivenReturnId_BreadCrumbIsSet()
        {
            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData() { Id = organisationId }
            };

            var result = await controller.Confirmation(returnData.Id) as ViewResult;

            var viewModel = result.Model as SubmittedReturnViewModel;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            Assert.Equal(organisationId, returnData.OrganisationData.Id);
            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async Task GetConfirmation_GivenReturnId_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var result = await controller.Confirmation(returnId) as ViewResult;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetConfirmation_GivenReturnId_ViewModelShouldBeMapped()
        {
            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData() { Id = organisationId }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            var result = await controller.Confirmation(A.Dummy<Guid>()) as ViewResult;

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(returnData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetConfirmation_GivenReturnId_MappedViewModelShouldBeReturnedToView()
        {
            var viewModel = new SubmittedReturnViewModel();

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(A<ReturnData>._)).Returns(viewModel);

            var result = await controller.Confirmation(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task PostConfirmation_RedirectsToChooseActivity()
        {
            var viewModel = new SubmittedReturnViewModel()
            {
                OrganisationId = organisationId
            };

            var result = await controller.Confirmation(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ChooseActivity");
            result.RouteValues["controller"].Should().Be("Home");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["area"].Should().Be("Scheme");
        }
    }
}
