namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class ObligatedSentOnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IObligatedSentOnWeeeRequestCreator requestCreator;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligatedSentOnController controller;
        private readonly IWeeeCache cache;
        private readonly IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel> mapper;
        private readonly ICategoryValueTotalCalculator calculator;

        public ObligatedSentOnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            requestCreator = A.Fake<IObligatedSentOnWeeeRequestCreator>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMap<ReturnToObligatedViewModelMapTransfer, ObligatedViewModel>>();
            calculator = A.Fake<ICategoryValueTotalCalculator>();

            controller = new ObligatedSentOnController(cache, breadcrumb, () => weeeClient, mapper, requestCreator);
        }

        [Fact]
        public void ObligatedSentOnControllerInheritsExternalSiteController()
        {
            typeof(ObligatedReusedController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void ObligatedSentOnController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(ObligatedReusedController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new ObligatedViewModel(calculator);
            var request = new AddObligatedSentOn();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");

            await controller.Index(A.Dummy<ObligatedViewModel>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddObligatedReused>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            const string orgName = "orgName";

            var @return = A.Fake<ReturnData>();
            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 3, 30), (int)Core.DataReturns.QuarterType.Q1);
            var aatfInfo = A.Fake<AatfData>();
            var aatfId = Guid.NewGuid();

            const string reportingQuarter = "2019 Q1 Jan - Mar";
            const string reportingPeriod = "Test (WEE/QW1234RE/ATF)";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;
            const string aatfName = "Test";
            aatfInfo.ApprovalNumber = "WEE/QW1234RE/ATF";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(orgName);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);
            A.CallTo(() => cache.FetchAatfData(organisationId, aatfId)).Returns(aatfInfo);
            A.CallTo(() => aatfInfo.Name).Returns(aatfName);

            await controller.Index(A.Dummy<Guid>(), organisationId, A.Dummy<Guid>(), aatfId, A.Dummy<String>());

            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfReturn);
            breadcrumb.ExternalOrganisation.Should().Be(orgName);

            Assert.Contains(reportingQuarter, breadcrumb.QuarterDisplayInfo);
            Assert.Contains(reportingPeriod, breadcrumb.AatfDisplayInfo);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<String>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_MapperShouldBeCalledWithCorrectParameters()
        {
            var siteName = "SiteName";
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var weeeSentOnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            /*var transfer = new ReturnToObligatedViewModelMapTransfer()
            {
                OrganisationId = organisationId,
                ReturnId = returnId,
                ReturnData = @return,
                AatfId = aatfId,
                OperatorName = operatorName,
                WeeeSentOnId = weeeSentOnId
            };*/

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(@return);

            await controller.Index(returnId, organisationId, weeeSentOnId, aatfId, siteName);

            A.CallTo(() => mapper.Map(A<ReturnToObligatedViewModelMapTransfer>.That.Matches(t => t.ReturnId.Equals(returnId) && t.AatfId.Equals(aatfId) && t.WeeeSentOnId.Equals(weeeSentOnId) && t.OrganisationId.Equals(organisationId) && t.SiteName.Equals(siteName) && t.ReturnData.Equals(@return)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_ViewModelShouldBeReturnedWithCorrectValues()
        {
            var operatorName = "OpName";
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var weeeSentOnId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            var viewModel = A.Fake<ObligatedViewModel>();

            A.CallTo(() => mapper.Map(A<ReturnToObligatedViewModelMapTransfer>._)).Returns(viewModel);

            var result = await controller.Index(returnId, organisationId, weeeSentOnId, aatfId, operatorName) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void IndexGet_ProvidedTempDataForCopyPasteValues_MapperShouldReturnViewModelWithCopyPasteValues()
        {
            string siteName = "SiteName";
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();
            Guid aatfId = Guid.NewGuid();
            Guid weeeSentOnId = Guid.NewGuid();
            ReturnData returnData = A.Fake<ReturnData>();
            string b2bContent = "1\r\n2\r\n3\r\n4\r\n";
            string b2cContent = "1\r\n2\r\n3\r\n4\r\n";

            ObligatedCategoryValue obligatedCategoryValue = new ObligatedCategoryValue() { B2B = b2bContent, B2C = b2cContent };

            TempDataDictionary tempdata = new TempDataDictionary();
            tempdata.Add("pastedValues", obligatedCategoryValue);

            controller.TempData = tempdata;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId.Equals(returnId)))).Returns(returnData);

            await controller.Index(returnId, organisationId, weeeSentOnId, aatfId, siteName);

            A.CallTo(() => mapper.Map(A<ReturnToObligatedViewModelMapTransfer>.That.Matches(t => t.ReturnId.Equals(returnId) && t.AatfId.Equals(aatfId) && t.WeeeSentOnId.Equals(weeeSentOnId) && t.OrganisationId.Equals(organisationId) && t.SiteName.Equals(siteName) && t.ReturnData.Equals(returnData) && t.PastedData == obligatedCategoryValue))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
