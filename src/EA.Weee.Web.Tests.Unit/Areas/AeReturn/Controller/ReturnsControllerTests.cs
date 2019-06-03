namespace EA.Weee.Web.Tests.Unit.Areas.AeReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AeReturn.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Returns;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
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
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>.That.Matches(r => r.OrganisationId.Equals(organisationId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeBuilt()
        {
            var returns = new List<ReturnData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturns>._)).Returns(returns);

            await controller.Index(A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<ReturnsViewModel>(returns)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenOrganisation_ReturnsViewModelShouldBeReturned()
        {
            var model = new ReturnsViewModel();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => mapper.Map<ReturnsViewModel>(A<IList<ReturnData>>._)).Returns(model);

            var result = await controller.Index(organisationId) as ViewResult;

            var returnedModel = (ReturnsViewModel)model;
            
            returnedModel.Should().Be(model);
            returnedModel.OrganisationId.Should().Be(organisationId);
        }
    }
}
