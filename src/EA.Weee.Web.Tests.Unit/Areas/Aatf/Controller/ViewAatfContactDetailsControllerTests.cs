namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoFixture;

    using EA.Weee.Api.Client;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class ViewAatfContactDetailsControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ViewAatfContactDetailsController controller;
        private readonly Fixture fixture;

        public ViewAatfContactDetailsControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.fixture = new Fixture();

            this.controller = new ViewAatfContactDetailsController(this.cache, this.breadcrumb, () => this.apiClient);
        }

        [Fact]
        public void HomeControllerInheritsExternalSiteController()
        {
            typeof(ViewAatfContactDetailsController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Ae, "AE")]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet(FacilityType facilityType, string expected)
        {
            var organisationName = "Organisation";
            var aatfDataExternal = new AatfDataExternal(Guid.NewGuid(), "AATF")
            {
                ApprovalNumber = "Approval",
                FacilityType = "AATF",
                Status = "Approved"
            };

            A.CallTo(() => this.cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);
            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfDataExternal);

            await this.controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), facilityType);

            this.breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            this.breadcrumb.ExternalAatf.Should().BeEquivalentTo(aatfDataExternal);
            this.breadcrumb.ExternalActivity.Should().Be($"View {expected} contact details");
        }

        [Fact]
        public async void IndexGet_GivenActionParameters_ApiShouldBeCalled()
        {
            var aatfId = this.fixture.Create<Guid>();

            await this.controller.Index(A.Dummy<Guid>(), aatfId, this.fixture.Create<FacilityType>());

            A.CallTo(() => this.apiClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task IndexGet_GivenActionParameters_HomeViewModelShouldBeBuiltAsync()
        {
            var organisationId = this.fixture.Create<Guid>();
            var aatfId = this.fixture.Create<Guid>();
            var facilityType = this.fixture.Create<FacilityType>();

            var result = await this.controller.Index(organisationId, aatfId, facilityType) as ViewResult;

            var model = result.Model as ViewAatfContactDetailsViewModel;

            model.FacilityType.Should().Be(facilityType);
            model.OrganisationId.Should().Be(organisationId);
            model.AatfId.Should().Be(aatfId);
        }
    }
}
