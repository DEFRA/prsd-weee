namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ViewAatfContactDetailsControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ViewAatfContactDetailsController controller;

        public ViewAatfContactDetailsControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();

            this.controller = new ViewAatfContactDetailsController(cache, breadcrumb, () => apiClient);
        }

        [Fact]
        public void HomeControllerInheritsExternalSiteController()
        {
            typeof(ViewAatfContactDetailsController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";

            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), false);

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
        }

        [Fact]
        public async void IndexGet_GivenAatfId_ApiShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();

            await controller.Index(A.Dummy<Guid>(), aatfId, false);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_HomeViewModelShouldBeBuilt()
        {
            var organisationId = Guid.NewGuid();

            var aatfId = Guid.NewGuid();

            var result = await controller.Index(organisationId, aatfId, false) as ViewResult;

            var model = result.Model as ViewAatfContactDetailsViewModel;

            model.IsAE.Should().Be(false);
            model.OrganisationId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }
    }
}
