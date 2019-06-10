namespace EA.Weee.Web.Tests.Unit.Areas.AeReturn.Controller
{
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
    using System.Web.Mvc;
    using Core.DataReturns;
    using Xunit;

    public class ReturnsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReturnsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly Guid organisationId;

        public ReturnsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            organisationId = Guid.NewGuid();

            controller = new ReturnsController(() => weeeClient, breadcrumb, A.Fake<IWeeeCache>(), mapper);
        }

        [Fact]
        public void ReturnsControllerInheritsAeReturnBaseController()
        {
            typeof(ReturnsController).BaseType.Name.Should().Be(typeof(AeReturnBaseController).Name);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_BreadcrumbShouldBeSet()
        {
            await controller.Index(A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsShouldBeRetrieved()
        {
            await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>.That.Matches(r => r.OrganisationId.Equals(organisationId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeBuilt()
        {
            var returnsData = new ReturnsData(A.Fake<List<ReturnData>>(), new Quarter(2019, QuarterType.Q1));

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returnsData);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnsViewModel>(returnsData)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeReturned()
        {
            var model = new ReturnsViewModel();

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<ReturnsData>._)).Returns(model);

            var result = await controller.Index(organisationId) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;
            
            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void GetExportedWholeWeee_GivenOrganisation_DefaultViewShouldBeReturned()
        {
            var result = await controller.ExportedWholeWeee(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void GetExportedWholeWeee_NoReturnIdProvided_ReturnCreated_ViewModelReturnIdShouldBePopulated()
        {
            var returnId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturn>._)).Returns(returnId);

            var result = await controller.ExportedWholeWeee(organisationId) as ViewResult;

            var viewModel = result.Model as ExportedWholeWeeeViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);
        }

        [Fact]
        public async void GetExportedWholeWeee_ReturnIdProvided_ReturnNotCreated_ViewModelReturnIdShouldBePopulated()
        {
            var returnId = Guid.NewGuid();

            var result = await controller.ExportedWholeWeee(organisationId, returnId) as ViewResult;

            var viewModel = result.Model as ExportedWholeWeeeViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturn>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void GetExportedWholeWeee_BreadCrumbShouldBeSet()
        {
            await controller.ExportedWholeWeee(A.Dummy<Guid>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Theory]
        [InlineData(YesNoEnum.Yes, "Index")]
        [InlineData(YesNoEnum.No, "NilReturn")]
        public async void PostExportedWholeWeee_SelectedValueGiven_CorrectRedirectHappens(YesNoEnum selectedValue, string action)
        {
            var viewModel = new ExportedWholeWeeeViewModel()
            {
                WeeeSelectedValue = selectedValue,
                ReturnId = Guid.NewGuid()
            };

            var result = await controller.ExportedWholeWeee(organisationId, viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
            
            if (selectedValue == YesNoEnum.No)
            {
                result.RouteValues["returnId"].Should().Be(viewModel.ReturnId);
            }
        }

        [Fact]
        public void IndexPost_RedirectToExportedWholeWeee()
        {
            var organisationId = Guid.NewGuid();

            var viewModel = new ReturnsViewModel()
            {
                OrganisationId = organisationId
            };

            var result = controller.Index(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("ExportedWholeWeee");
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
        }

        [Fact]
        public async void PostExportedWholeWeee_NoValueSelected_ReturnsViewWithViewModelAndBreadCrumbSet()
        {
            var viewModel = new ExportedWholeWeeeViewModel();

            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.ExportedWholeWeee(organisationId, viewModel) as ViewResult;

            result.ViewName.Should().BeEmpty();
            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async void GetNilResult_BreadCrumbSet_ViewReturnedWithModel()
        {
            var returnId = Guid.NewGuid();

            var result = await controller.NilReturn(organisationId, returnId) as ViewResult;

            var viewModel = result.Model as NilReturnViewModel;

            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(returnId, viewModel.ReturnId);
            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async void PostNilResult_RedirectToReturnsList()
        {
            var viewModel = new NilReturnViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = Guid.NewGuid()
            };

            var result = await controller.NilReturnConfirm(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Confirmation");
            result.RouteValues["controller"].Should().Be("Returns");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteName.Should().Be(AeRedirect.ReturnsRouteName);
        }

        [Fact]
        public async void PostNilResult_SubmitReturnShouldBeCalled()
        {
            var viewModel = new NilReturnViewModel()
            {
                OrganisationId = organisationId,
                ReturnId = Guid.NewGuid()
            };

            await controller.NilReturnConfirm(viewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitReturn>.That.Matches(c => c.ReturnId.Equals(viewModel.ReturnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetConfirmation_BreadCrumbIsSet_ReturnedWithViewModel()
        {
            var result = await controller.Confirmation(organisationId) as ViewResult;

            var viewModel = result.Model as ConfirmationViewModel;

            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AeReturn);
        }

        [Fact]
        public async void PostConfirmation_RedirectsToChooseActivity()
        {
            var viewModel = new ConfirmationViewModel()
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
