namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
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
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Requests.Shared;
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
        public void SentOnCreateSiteControllerInheritsExternalSiteController()
        {
            typeof(SentOnCreateSiteController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public void SentOnCreateSiteController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SentOnCreateSiteController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();
            var organisationData = A.Fake<OrganisationData>();
            const string orgName = "orgName";

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30));
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

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

            await controller.Index(Guid.NewGuid(), aatfId, null, null);

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), null, null) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenWeeeSentOnId_ApiShouldBeCalled()
        {
            var weeeSentOnId = Guid.NewGuid();

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), weeeSentOnId, null);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOnById>.That.Matches(w => w.WeeeSentOnId == weeeSentOnId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturnId_ApiShouldBeCalled()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId, A.Dummy<Guid>(), A.Dummy<Guid>(), null);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenWeenSentOn_MapperShouldBeCalled()
        {
            var weeeSentOn = A.Fake<WeeeSentOnData>();
            var returnData = A.Fake<ReturnData>();
            var countryData = A.Fake<List<CountryData>>();

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetWeeeSentOnById>._)).Returns(weeeSentOn);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);
            A.CallTo(() => apiClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countryData);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), null);

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer>.That.Matches(r =>
                    r.Return.Equals(returnData) && r.WeeeSentOnData.Equals(weeeSentOn) && r.CountryData.Equals(countryData))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenMappedViewModel_ViewModelShouldBeReturned()
        {
            var model = A.Fake<SentOnCreateSiteViewModel>();

            A.CallTo(() => mapper.Map(A<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), null) as ViewResult;

            model.Should().Be(model);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            var form = new FormCollection();
            controller.ModelState.AddModelError("error", "error");
            var model = new SentOnCreateSiteViewModel
            {
                SiteAddressData = new AatfAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST"),
                OperatorAddressData = new OperatorAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST")
            };
            await controller.Index(model, form);

            A.CallTo(() => apiClient.SendAsync(A<string>._, A<AddSentOnAatfSite>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var form = new FormCollection();
            var model = new SentOnCreateSiteViewModel
            {
                SiteAddressData = new AatfAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST")
            };
            var request = new AddSentOnAatfSite();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model, form);

            A.CallTo(() => apiClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        public async void IndexPost_GivenValidViewModel_IsOperatorTheSameAsAATFShouldBeSet(string operatorBool)
        {
            var form = new FormCollection();
            var boolConversion = Convert.ToBoolean(operatorBool);
            var model = new SentOnCreateSiteViewModel
            {
                OrganisationId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                WeeeSentOnId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                OperatorAddressData = new OperatorAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST"),
                SiteAddressData = new AatfAddressData("TEST", "TEST", "TEST", "TEST", "TEST", "TEST", Guid.NewGuid(), "TEST")
            };

            form.Add("IsOperatorTheSameAsAATF", operatorBool);

            await controller.Index(model, form);

            A.CallTo(() => requestCreator.ViewModelToRequest(A<SentOnCreateSiteViewModel>.That.Matches(m => m.IsOperatorTheSameAsAatf == boolConversion))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
