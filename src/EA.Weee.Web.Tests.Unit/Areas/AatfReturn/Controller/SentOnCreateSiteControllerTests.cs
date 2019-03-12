namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
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
    using System.Web.Mvc;
    using Xunit;

    public class SentOnCreateSiteControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddSentOnAatfSiteRequestCreator requestCreator;
        private readonly SentOnCreateSiteController controller;
        private readonly IMap<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer, SentOnCreateSiteViewModel> mapper;

        public SentOnCreateSiteControllerTests()
        {
            apiClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddSentOnAatfSiteRequestCreator>();
            mapper = A.Fake<IMap<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer, SentOnCreateSiteViewModel>>();

            controller = new SentOnCreateSiteController(() => apiClient, breadcrumb, cache, requestCreator, mapper);
        }

        [Fact]
        public void CheckReuseOffSiteCreateSiteControllerInheritsExternalSiteController()
        {
            typeof(SentOnCreateSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var operatorData = A.Fake<OperatorData>();
            const string orgName = "orgName";

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => operatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => @return.ReturnOperatorData).Returns(operatorData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            await controller.Index(Guid.NewGuid(), Guid.NewGuid());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var model = new SentOnCreateSiteViewModel();

            A.CallTo(() => mapper.Map(new ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer() { ReturnId = Guid.NewGuid(), AatfId = Guid.NewGuid(), OrganisationId = Guid.NewGuid() })).Returns(model);
            model.SiteAddressData = new AddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST");

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");
            var model = new SentOnCreateSiteViewModel();
            model.SiteAddressData = new AddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST");
            await controller.Index(model);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<AddSentOnAatfSite>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new SentOnCreateSiteViewModel();
            model.SiteAddressData = new AddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST");
            var request = new AddSentOnAatfSite();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => apiClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
