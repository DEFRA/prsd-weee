namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Attributes;
    using Xunit;

    public class SelectYourPcsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly SelectYourPcsController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        public List<SchemeData> SchemeList;
        private readonly IAddReturnSchemeRequestCreator requestCreator;

        public SelectYourPcsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            requestCreator = A.Fake<IAddReturnSchemeRequestCreator>();

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
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexPost_GivenModel_RedirectShouldBeCorrect()
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
        public async void IndexGet_ReselectIsTrue_ReselectActionCalledAndViewReturned()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();

            ViewResult result = await controller.Index(organisationId, returnId, true) as ViewResult;

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Reselect");

            SelectYourPcsViewModel viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(returnId, viewModel.ReturnId);
            Assert.Equal(organisationId, viewModel.OrganisationId);
        }

        [Fact]
        public async void IndexGet_ReselectIsTrue_CallsToGetExistingSelectedSchemesMustHaveBeenCalledAndViewModelListPopulatedWithGuids()
        {
            Guid returnId = Guid.NewGuid();
            Guid organisationId = Guid.NewGuid();

            List<Guid> selectedSchemeIds = new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            SchemeDataList schemeDataList = new SchemeDataList()
            {
                SchemeDataItems = new List<SchemeData>()
            };

            foreach (Guid scheme in selectedSchemeIds)
            {
                schemeDataList.SchemeDataItems.Add(new SchemeData() { Id = scheme });
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).Returns(schemeDataList);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(A.Fake<List<SchemeData>>());

            ViewResult result = await controller.Index(organisationId, returnId, true) as ViewResult;

            SelectYourPcsViewModel viewModel = result.Model as SelectYourPcsViewModel;

            Assert.Equal(selectedSchemeIds, viewModel.SelectedSchemes);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ReselectPost_SchemesRemoved()
        {
            Guid returnId = Guid.NewGuid();

            SelectYourPcsViewModel model = new SelectYourPcsViewModel()
            {
                ReturnId = returnId
            };

            List<SchemeData> existingSchemes = A.CollectionOfDummy<SchemeData>(3).ToList();
            List<Guid> reselectedSchemes = new List<Guid>();

            foreach (SchemeData scheme in existingSchemes)
            {
                scheme.Id = Guid.NewGuid();
                reselectedSchemes.Add(scheme.Id);
            }

            model.SchemeList = existingSchemes;

            SchemeDataList usersAlreadySavedSchemeDataList = new SchemeDataList()
            {
                SchemeDataItems = existingSchemes
            };

            List<Guid> removedPcs = new List<Guid>()
            {
                reselectedSchemes[reselectedSchemes.Count - 1]
            };

            reselectedSchemes.RemoveAt(reselectedSchemes.Count - 1);

            model.SelectedSchemes = reselectedSchemes;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).Returns(usersAlreadySavedSchemeDataList);

            ViewResult result = await controller.Index(model, true) as ViewResult;
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "PcsRemoved");

            PcsRemovedViewModel resultModel = result.Model as PcsRemovedViewModel;

            Assert.Equal(removedPcs, resultModel.RemovedSchemeList.Select(p => p.Id));
            Assert.Equal(model.ReturnId, returnId);
        }

        [Fact]
        public async void ReselectPost_NoSchemeRemoved_RedirectToTaskListAndAddSchemeRequestSentForEachScheme()
        {
            Guid returnId = Guid.NewGuid();

            SelectYourPcsViewModel model = new SelectYourPcsViewModel()
            {
                ReturnId = returnId
            };

            List<SchemeData> existingSchemes = A.CollectionOfDummy<SchemeData>(2).ToList();
            List<Guid> reselectedSchemes = new List<Guid>();

            foreach (SchemeData scheme in existingSchemes)
            {
                scheme.Id = Guid.NewGuid();
                reselectedSchemes.Add(scheme.Id);
            }

            reselectedSchemes.Add(Guid.NewGuid());

            SchemeDataList usersAlreadySavedSchemeDataList = new SchemeDataList()
            {
                SchemeDataItems = existingSchemes
            };

            model.SchemeList = existingSchemes;
            model.SelectedSchemes = reselectedSchemes;

            List<AddReturnScheme> requests = new List<AddReturnScheme>()
            {
                new AddReturnScheme()
                {
                    ReturnId = model.ReturnId,
                    SchemeId = reselectedSchemes[2]
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturnScheme>.That.Matches(p => p.ReturnId == returnId))).Returns(usersAlreadySavedSchemeDataList);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturnScheme>.That.Matches(p => p.ReturnId == returnId && p.SchemeId == reselectedSchemes[2])));

            RedirectToRouteResult result = await controller.Index(model, true) as RedirectToRouteResult;

            result.RouteValues["returnId"].Should().Be(returnId);
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteName.Should().Be(AatfRedirect.Default);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddReturnScheme>.That.Matches(p => p.ReturnId == returnId && p.SchemeId == reselectedSchemes[0]))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}