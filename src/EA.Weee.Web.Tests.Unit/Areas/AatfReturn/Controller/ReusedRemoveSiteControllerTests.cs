namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
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
    using Xunit;

    public class ReusedRemoveSiteControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ReusedRemoveSiteController controller;

        public ReusedRemoveSiteControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();

            controller = new ReusedRemoveSiteController(() => apiClient, breadcrumb, cache);
        }

        [Fact]
        public void CheckSentOnCreateSiteOperatorControllerInheritsExternalSiteController()
        {
            typeof(ReusedRemoveSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var siteId = Guid.NewGuid();
            var schemeInfo = A.Fake<SchemePublicInfo>();
            const string orgName = "orgName";
            var addressTonnage = new AddressTonnageSummary()
            {
                AddressData = (List<SiteAddressData>)A.CollectionOfFake<SiteAddressData>(2)
            };

            A.CallTo(() => apiClient.SendAsync(A<string>._, new GetAatfSite(A<Guid>._, A<Guid>._))).Returns(addressTonnage);
            //A.CallTo(() => addressTonnage.AddressData.Select(s => s).Where(s => s.Id == siteId).Single()).Returns(siteAddress);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(organisationId, A.Dummy<Guid>(), A.Dummy<Guid>(), siteId);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexPost_GivenSelectedValueIsYes_RemoveAatfSiteIsCalled()
        {
            var viewModel = new ReusedRemoveSiteViewModel()
            {
                SelectedValue = "Yes",
                SiteId = Guid.NewGuid()
            };

            await controller.Index(viewModel);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<RemoveAatfSite>.That.Matches(r => r.SiteId == viewModel.SiteId))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
