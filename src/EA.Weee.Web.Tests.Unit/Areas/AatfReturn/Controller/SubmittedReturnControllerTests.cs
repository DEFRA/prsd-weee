namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using Core.Scheme;
    using EA.Weee.Core.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using TestHelpers;
    using Web.Areas.AatfReturn.Attributes;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class SubmittedReturnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SubmittedReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;
        private readonly IWeeeCache cache;

        public SubmittedReturnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            cache = A.Fake<IWeeeCache>();

            controller = new SubmittedReturnController(() => weeeClient, cache, breadcrumb, mapper);
        }

        [Fact]
        public void SubmittedReturnControllerInheritsExternalSiteController()
        {
            typeof(SubmittedReturnController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckReturnViewModelShouldBeBuilt()
        {
            var @return = A.Fake<ReturnData>();
            A.CallTo(() => @return.OrganisationData).Returns(A.Fake<OrganisationData>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(@return)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckReturnViewModelShouldBeReturned()
        {
            var model = A.Fake<SubmittedReturnViewModel>();

            A.CallTo(() => mapper.Map<SubmittedReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
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

            var returnId = Guid.NewGuid();

            await controller.Index(returnId);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async void IndexPost_GivenModel_RedirectShouldBeCorrect()
        {
            var model = new SubmittedReturnViewModel() { OrgansationId = Guid.NewGuid() };
            var redirect = await controller.Index(model) as RedirectToRouteResult;

            redirect.RouteValues["action"].Should().Be("ChooseActivity");
            redirect.RouteValues["controller"].Should().Be("Home");
            redirect.RouteValues["area"].Should().Be("Scheme");
            redirect.RouteValues["pcsId"].Should().Be(model.OrgansationId);
        }
    }
}
