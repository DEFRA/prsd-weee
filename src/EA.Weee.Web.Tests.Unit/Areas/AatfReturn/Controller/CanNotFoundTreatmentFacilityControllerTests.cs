namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class CanNotFoundTreatmentFacilityControllerTests
    {
        private readonly IWeeeClient apiClient;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer, CanNotFoundTreatmentFacilityViewModel> mapper;
        private readonly CanNotFoundTreatmentFacilityController controller;        

        public CanNotFoundTreatmentFacilityControllerTests()
        {
            this.apiClient = A.Fake<IWeeeClient>();
            this.breadcrumb = A.Fake<BreadcrumbService>();
            this.cache = A.Fake<IWeeeCache>();
            this.mapper = A.Fake<IMap<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer, CanNotFoundTreatmentFacilityViewModel>>();

            controller = new CanNotFoundTreatmentFacilityController(() => apiClient, breadcrumb, cache, mapper);
        }

        [Fact]
        public void CheckCanNotFoundTreatmentFacilityControllerInheritsFromExternalSiteController()
        {
            typeof(CanNotFoundTreatmentFacilityController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void CheckCanNotFoundTreatmentFacilityController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(CanNotFoundTreatmentFacilityController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            const string selectedAatfName = "Test";

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => organisationData.Id).Returns(organisationId);
            A.CallTo(() => @return.OrganisationData).Returns(organisationData);
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(@return.Id, aatfId, selectedAatfName);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }        

        [Fact]
        public async void IndexGet_GivenActionAndParameters_CanNotFoundTreatmentFacilityViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var selectedAatfName = "Test";            

            var model = new CanNotFoundTreatmentFacilityViewModel()
            {
                AatfId = aatfId,
                ReturnId = returnId,
                OrganisationId = organisationId                
            };

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(returnId, aatfId, selectedAatfName) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CanNotFoundTreatmentFacilityViewModelShouldBeBuilt()
        {
            var weeeSentOnList = A.Fake<List<WeeeSentOnData>>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOn>._)).Returns(weeeSentOnList);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer>._)).MustHaveHappened(Repeated.Exactly.Once);
        }        
    }
}
