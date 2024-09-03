namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class SelectYourPcsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectYourPcsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IAddReturnSchemeRequestCreator requestCreator;
        private readonly Fixture fixture;

        public SelectYourPcsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddReturnSchemeRequestCreator>();
            fixture = new Fixture();

            controller = new SelectYourPcsController(() => weeeClient, breadcrumb, cache, requestCreator);
        }

        [Fact]
        public void SelectYourPcsControllerInheritsExternalSiteController()
        {
            typeof(SelectYourPcsController).BaseType.Name.Should().Be(typeof(AatfReturnBaseController).Name);
        }

        [Fact]
        public void SelectYourPcsController_ShouldHaveValidateOrganisationActionFilterAttribute()
        {
            typeof(SelectYourPcsController).Should().BeDecoratedWith<ValidateOrganisationActionFilterAttribute>();
        }

        [Fact]
        public void SelectYourPcsController_ShouldHaveValidateReturnActionFilterAttribute()
        {
            typeof(SelectYourPcsController).Should().BeDecoratedWith<ValidateReturnCreatedActionFilterAttribute>();
        }

        [Fact]
        public async Task IndexGet_GiveActionExecutes_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            await controller.Index(organisationId, returnId);
            SystemTime.Unfreeze();

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async Task IndexPost_GivenModel_RedirectShouldBeCorrect()
        {
            var returnId = Guid.NewGuid();

            var viewModel = new SelectYourPcsViewModel(A.Fake<List<SchemeData>>(), A.Fake<List<Guid>>())
            {
                ReturnId = returnId
            };

            var redirect = await controller.Index(viewModel) as RedirectToRouteResult;

            redirect.RouteValues["action"].Should().Be("Index");
            redirect.RouteValues["controller"].Should().Be("AatfTaskList");
            redirect.RouteValues["returnId"].Should().Be(returnId);
        }

        [Fact]
        public async Task IndexGet_ReselectIsTrue_ReselectActionCalledAndViewReturned()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var result = await controller.Index(organisationId, returnId, true) as ViewResult;

            Assert.True(result.ViewName == "Index");

            var viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);
            Assert.Equal(organisationId, viewModel.OrganisationId);
        }

        [Fact]
        public async Task IndexGet_CopyPreviousIsTrue_ViewModelSelectedPcsSelectedAndViewReturned()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var schemeDatas = A.CollectionOfFake<SchemeData>(4).ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeDatas);

            var selectedSchemes = new List<Guid>()
            {
                schemeDatas.FirstOrDefault().Id
            };

            var q = new Quarter(2019, QuarterType.Q1);

            var previousQuarterReturnResult = new PreviousQuarterReturnResult()
            {
                PreviousSchemes = selectedSchemes,
                PreviousQuarter = q
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPreviousQuarterSchemes>._)).Returns(previousQuarterReturnResult);

            var result = await controller.Index(organisationId, returnId, false, true) as ViewResult;

            Assert.True(result.ViewName == "Index");

            var viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);
            Assert.Equal(organisationId, viewModel.OrganisationId);
            viewModel.CopyPrevious.Should().Be(true);
            viewModel.SelectedSchemes.Should().Contain(selectedSchemes);
            viewModel.PreviousQuarterData.PreviousQuarter.Should().Be(q);
        }

        [Fact]
        public async Task IndexGet_ClearSelectionsIsTrue_ViewModelSelectedPcsIsEmptyAndViewReturned()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var result = await controller.Index(organisationId, returnId, false, false, true) as ViewResult;

            Assert.True(result.ViewName == "Index");

            var viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);
            Assert.Equal(organisationId, viewModel.OrganisationId);

            viewModel.SelectedSchemes.Count().Should().Be(0);
        }

        [Fact]
        public async Task IndexGet_ReselectIsTrue_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var @return = A.Fake<ReturnData>();

            var quarterData = new Quarter(2019, QuarterType.Q1);
            var quarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            const string reportingPeriod = "2019 Q1 Jan - Mar";
            @return.Quarter = quarterData;
            @return.QuarterWindow = quarterWindow;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            await controller.Index(organisationId, returnId, true);
            SystemTime.Unfreeze();

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async Task IndexGet_ReselectIsTrue_CallsToGetExistingSelectedSchemesMustHaveBeenCalledAndViewModelListPopulatedWithGuids()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var selectedSchemeIds = new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var schemeDataList = new SchemeDataList()
            {
                SchemeDataItems = new List<SchemeData>()
            };

            foreach (var scheme in selectedSchemeIds)
            {
                schemeDataList.SchemeDataItems.Add(new SchemeData() { Id = scheme });
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).Returns(schemeDataList);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(A.Fake<List<SchemeData>>());

            var result = await controller.Index(organisationId, returnId, true) as ViewResult;

            var viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(selectedSchemeIds, viewModel.SelectedSchemes);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexPost_GivenSchemesRemoved_ShouldRedirectToRemovedPcsRoute()
        {
            var existingSchemes = A.CollectionOfDummy<SchemeData>(3).ToList();
            var reselectedSchemes = new List<Guid>();

            foreach (var scheme in existingSchemes)
            {
                scheme.Id = Guid.NewGuid();
                reselectedSchemes.Add(scheme.Id);
            }

            var usersAlreadySavedSchemeDataList = new SchemeDataList()
            {
                SchemeDataItems = existingSchemes
            };

            var removedItem = reselectedSchemes.ElementAt(reselectedSchemes.Count - 1);
            reselectedSchemes.RemoveAt(reselectedSchemes.Count - 1);

            var model = new SelectYourPcsViewModel()
            {
                ReturnId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                SchemeList = existingSchemes,
                SelectedSchemes = reselectedSchemes
            };

            model.SelectedSchemes = reselectedSchemes;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == model.ReturnId))).Returns(usersAlreadySavedSchemeDataList);

            var result = await controller.Index(model, true) as RedirectToRouteResult;

            result.RouteName.Should().Be(AatfRedirect.RemovedPcsRouteName);
            result.RouteValues["returnId"].Should().Be(model.ReturnId);
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);

            var removedSchemeList = controller.TempData["RemovedSchemeList"] as List<SchemeData>;
            removedSchemeList.Count.Should().Be(1);
            removedSchemeList.ElementAt(0).Id.Should().Be(removedItem);

            controller.TempData["SelectedSchemes"].Should().Be(model.SelectedSchemes);

            var removedSchemes = controller.TempData["RemovedSchemes"] as List<Guid>;
            removedSchemes.Count().Should().Be(1);
            removedSchemes.ElementAt(0).Should().Be(removedItem);
        }

        [Fact]
        public async Task IndexPost_NoSchemeRemoved_RedirectToTaskListAndAddSchemeRequestSentForEachScheme()
        {
            var returnId = Guid.NewGuid();

            var reselectedSchemes = PrepareSaveSchemes(returnId);

            var model = new SelectYourPcsViewModel()
            {
                ReturnId = returnId,
                SelectedSchemes = reselectedSchemes
            };

            var result = await controller.Index(model, true) as RedirectToRouteResult;

            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteName.Should().Be(AatfRedirect.Default);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturnScheme>.That.Matches(p => p.ReturnId == returnId && p.SchemeIds == reselectedSchemes))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task IndexPost_GivenInvalidModelState_PreviousQuarterSelectionShouldBeRetrieved()
        {
            var model = new SelectYourPcsViewModel()
            {
                ReturnId = this.fixture.Create<Guid>(),
                OrganisationId = this.fixture.Create<Guid>()
            };

            var previousQuarterReturnResult = this.fixture.Build<PreviousQuarterReturnResult>()
                .With(p => p.PreviousQuarter, new Quarter(2019, QuarterType.Q1)).Create();

            A.CallTo(
                    () => weeeClient.SendAsync(
                        A<string>._,
                        A<GetPreviousQuarterSchemes>.That.Matches(p => p.ReturnId == model.ReturnId && p.OrganisationId == model.OrganisationId)))
                .Returns(previousQuarterReturnResult);

            this.controller.ModelState.AddModelError("error", "error");

            var result = await controller.Index(model, true) as ViewResult;

            var returnedModel = result.Model as SelectYourPcsViewModel;
            returnedModel.PreviousQuarterData.Should().Be(previousQuarterReturnResult);
        }

        [Fact]
        public async Task PcsRemovedGet_GiveActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await this.controller.PcsRemoved(this.fixture.Create<Guid>(), this.fixture.Create<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task PcsRemovedGet_GiveActionExecutes_ModelShouldBeReturned()
        {
            var organisationId = this.fixture.Create<Guid>();
            var returnId = this.fixture.Create<Guid>();
            var schemeData = this.fixture.CreateMany<SchemeData>().ToList();
            var selectedSchemes = this.fixture.CreateMany<Guid>().ToList();
            var removedSchemes = this.fixture.CreateMany<Guid>().ToList();

            controller.TempData["RemovedSchemeList"] = schemeData;
            controller.TempData["SelectedSchemes"] = selectedSchemes;
            controller.TempData["RemovedSchemes"] = removedSchemes;

            var result = await this.controller.PcsRemoved(organisationId, returnId) as ViewResult;

            var model = result.Model as PcsRemovedViewModel;
            model.OrganisationId.Should().Be(organisationId);
            model.ReturnId.Should().Be(returnId);
            model.RemovedSchemeList.Should().BeSameAs(schemeData);
            model.RemovedSchemes.Should().BeSameAs(removedSchemes);
            model.SelectedSchemes.Should().BeSameAs(selectedSchemes);
        }

        [Fact]
        public async Task PcsRemovedGet_GiveActionExecutes_BreadCrumbShouldBeSet()
        {
            var returnId = this.fixture.Create<Guid>();
            var organisationId = this.fixture.Create<Guid>();
            var @return = this.fixture.Build<ReturnData>().With(r => r.Quarter, new Quarter(2019, QuarterType.Q1)).With(
                r => r.QuarterWindow,
                QuarterWindowTestHelper.GetDefaultQuarterWindow()).Create();

            const string reportingPeriod = "2019 Q1 Jan - Mar";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(r => r.ReturnId == returnId && r.ForSummary == false))).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 04, 01));
            await controller.PcsRemoved(organisationId, returnId);
            SystemTime.Unfreeze();

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);

            Assert.Contains(reportingPeriod, breadcrumb.QuarterDisplayInfo);
        }

        [Fact]
        public async Task PcsRemovedPost_ModelStateNotValid_ReturnsViewWithViewModel()
        {
            var viewModel = A.Dummy<PcsRemovedViewModel>();

            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.PcsRemoved(viewModel) as ViewResult;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "PcsRemoved");

            var resultModel = result.Model as PcsRemovedViewModel;

            Assert.Equal(viewModel, resultModel);
        }

        [Fact]
        public async Task PcsRemovedPost_YesSelectedValue_ReturnsUserToTaskListCallsToSaveSchemesMade()
        {
            var returnId = Guid.NewGuid();

            var reselectedSchemes = PrepareSaveSchemes(returnId);

            var model = new PcsRemovedViewModel()
            {
                ReturnId = returnId,
                SelectedValue = "Yes",
                SelectedSchemes = reselectedSchemes,
                RemovedSchemes = new List<Guid>()
                {
                    Guid.NewGuid()
                }
            };

            var result = await controller.PcsRemoved(model) as RedirectToRouteResult;

            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteName.Should().Be(AatfRedirect.Default);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturnScheme>.That.Matches(p => p.ReturnId == returnId && p.SchemeIds == reselectedSchemes))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveReturnScheme>.That.Matches(p => p.SchemeIds == model.RemovedSchemes && p.ReturnId == returnId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task PcsRemovedPost_NoSelectedValue_ReturnsToSelectPcs()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var reselectedSchemes = PrepareSaveSchemes(returnId);

            var model = new PcsRemovedViewModel()
            {
                ReturnId = returnId,
                SelectedValue = "No",
                SelectedSchemes = reselectedSchemes,
                RemovedSchemes = new List<Guid>(),
                OrganisationId = organisationId
            };

            var result = await controller.PcsRemoved(model) as RedirectToRouteResult;

            result.RouteValues["organisationId"].Should().Be(organisationId);
            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["action"].Should().Be("Index");
            result.RouteName.Should().Be(AatfRedirect.SelectPcsRouteName);
        }

        private List<Guid> PrepareSaveSchemes(Guid returnId)
        {
            var reselectedSchemes = new List<Guid>();
            var existingSchemes = A.CollectionOfDummy<SchemeData>(2).ToList();

            foreach (var scheme in existingSchemes)
            {
                scheme.Id = Guid.NewGuid();
                reselectedSchemes.Add(scheme.Id);
            }

            reselectedSchemes.Add(Guid.NewGuid());

            var usersAlreadySavedSchemeDataList = new SchemeDataList()
            {
                SchemeDataItems = existingSchemes
            };

            var requests = new List<AddReturnScheme>()
            {
                new AddReturnScheme()
                {
                    ReturnId = returnId,
                    SchemeIds = reselectedSchemes
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).Returns(usersAlreadySavedSchemeDataList);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturnScheme>.That.Matches(p => p.ReturnId == returnId && p.SchemeIds == reselectedSchemes)));

            return reselectedSchemes;
        }
    }
}