namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ReceivedPcsListControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly ReceivedPcsListController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel> mapper;
        private readonly IWeeeCache cache;

        public ReceivedPcsListControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel>>();
            cache = A.Fake<IWeeeCache>();

            controller = new ReceivedPcsListController(() => weeeClient, cache, breadcrumb, mapper);
        }

        [Fact]
        public void ReceivedPcsListControllerInheritsExternalSiteController()
        {
            typeof(ReceivedPcsListController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ReceivedPcsListController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ReceivedPcsListController).Should().BeDecoratedWith<ValidateReturnActionFilterAttribute>();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var schemeData = A.Fake<SchemeDataList>();
            var schemeInfo = A.Fake<SchemePublicInfo>();
            var operatorData = A.Fake<OperatorData>();
            const string orgName = "orgName";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>._)).Returns(schemeData);
            A.CallTo(() => operatorData.OrganisationId).Returns(organisationId);
            A.CallTo(() => schemeData.OperatorData).Returns(operatorData);
            A.CallTo(() => schemeData.SchemeDataItems).Returns(A.Fake<List<SchemeData>>());
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);
            breadcrumb.SchemeInfo.Should().Be(schemeInfo);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var schemeList = A.Fake<SchemeDataList>();
            var returnId = Guid.NewGuid();

            A.CallTo(() => schemeList.OperatorData).Returns(A.Fake<OperatorData>());
            A.CallTo(() => schemeList.SchemeDataItems).Returns(A.Fake<List<SchemeData>>());

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var schemeList = A.Fake<SchemeDataList>();
            var returnId = Guid.NewGuid();

            A.CallTo(() => schemeList.OperatorData).Returns(A.Fake<OperatorData>());
            A.CallTo(() => schemeList.SchemeDataItems).Returns(A.Fake<List<SchemeData>>());

            await controller.Index(returnId, A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(g => g.ReturnId.Equals(returnId))))
            .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_ReceivedPcsListViewModelShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            const string aatfname = "aatfName";
            var receivedPcsData = A.Fake<List<ReceivedPcsData>>();

            var model = new ReceivedPcsListViewModel()
            {
                AatfId = aatfId,
                AatfName = aatfname,
                ReturnId = returnId,
                OrganisationId = organisationId,
                SchemeList = receivedPcsData
            };

            A.CallTo(() => mapper.Map(A<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer>._)).Returns(model);

            var result = await controller.Index(returnId, aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexGet_GivenReturn_ReceivedPcsListViewModelShouldBeBuilt()
        {
            var @return = A.Fake<ReturnData>();

            A.CallTo(() => @return.ReturnOperatorData).Returns(A.Fake<OperatorData>());
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => mapper.Map(A<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_OnSubmit_PageRedirectsToAatfTaskList()
        {
            var model = new ReceivedPcsListViewModel();
            var returnId = new Guid();
            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["area"].Should().Be("AatfReturn");
            result.RouteValues["returnId"].Should().Be(returnId);
        }
    }
}
