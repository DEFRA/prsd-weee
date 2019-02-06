namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.Requests;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn.NonObligated;
    using Xunit;

    public class SubmittedReturnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SubmittedReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public SubmittedReturnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new SubmittedReturnController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckSubmittedReturnControllerInheritsExternalSiteController()
        {
            typeof(SubmittedReturnController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_SubmittedReturnViewModelShouldBeReturned()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var result = await controller.Index(organisationId, returnId) as ViewResult;
            result.Should().NotBeNull();

            SubmittedReturnViewModel viewModel = result.Model as SubmittedReturnViewModel;
            result.Should().NotBeNull();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }
    }
}
