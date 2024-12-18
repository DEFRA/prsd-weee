﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class ProducersControllerTests : SimpleUnitTestBase
    {
        private readonly BreadcrumbService breadcumbService;
        private readonly ISearcher<ProducerSearchResult> producerSearcher;
        private readonly ISearcher<SmallProducerSearchResult> smallProducerSearcher;
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache cache;
        private readonly ConfigurationService configurationService;
        private readonly ProducersController controller;

        public ProducersControllerTests()
        {
            breadcumbService = A.Fake<BreadcrumbService>();
            producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            weeeClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            configurationService = A.Fake<ConfigurationService>();
            smallProducerSearcher = A.Fake<ISearcher<SmallProducerSearchResult>>();

            A.CallTo(() => configurationService.CurrentConfiguration.MaximumProducerOrganisationSearchResults).Returns(10);

            controller = new ProducersController(
                breadcumbService,
                producerSearcher,
                () => weeeClient,
                cache,
                configurationService,
                smallProducerSearcher);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ProducersController).Should().BeDerivedFrom<AdminController>();
        }

        [Theory]
        [InlineData(SearchTypeEnum.SmallProducer)]
        [InlineData(SearchTypeEnum.Producer)]

        public async Task GetSearch_ReturnsSearchView(SearchTypeEnum searchType)
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            // Act
            ActionResult result = await controller.Search(searchType);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");

            var model = viewResult.Model as SearchViewModel;
            model.SearchType.Should().Be(searchType);
        }

        [Fact]
        public async Task PostSearch_WithInvalidModel_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            SearchViewModel viewModel = new SearchViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithSearchTermAndNoSelectedPRN_RedirectsToSearchResultsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            SearchViewModel viewModel = new SearchViewModel { SearchTerm = "testSearchTerm", SelectedRegistrationNumber = null };

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Theory]
        [InlineData(SearchTypeEnum.SmallProducer, "ProducerSubmission", "Submissions")]
        [InlineData(SearchTypeEnum.Producer, "Producers", "Details")]
        public async Task PostSearch_WithSelectedPRN_RedirectsToDetailsAction(SearchTypeEnum searchTypeEnum, string expectedController, string expectedAction)
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            SearchViewModel viewModel = new SearchViewModel
            {
                SearchTerm = "testSearchTerm, WEE/AA1111AA",
                SelectedRegistrationNumber = "WEE/AA1111AA",
                SearchType = searchTypeEnum
            };

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal(expectedAction, redirectResult.RouteValues["action"]);
            Assert.Equal(expectedController, redirectResult.RouteValues["controller"]);
            Assert.Equal("WEE/AA1111AA", redirectResult.RouteValues["RegistrationNumber"]);
        }

        [Fact]
        public async Task PostSearchResults_WithInvalidModel_DoesSearchForTenResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            List<ProducerSearchResult> fakeResults = new List<ProducerSearchResult>()
            {
                new ProducerSearchResult()
                {
                    Name = "Producer1",
                    RegistrationNumber = "WEE/AA1111AA"
                }
            };

            ISearcher<ProducerSearchResult> producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            A.CallTo(() => producerSearcher.Search("testSearchTerm", 10, false))
                .Returns(fakeResults);

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            SearchResultsViewModel viewModel = new SearchResultsViewModel { SearchTerm = "testSearchTerm" };
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.SearchResults(viewModel);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            SearchResultsViewModel resultsViewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Contains(resultsViewModel.Results, r => r.RegistrationNumber == "WEE/AA1111AA");
        }

        [Fact]
        public async Task PostSearchResults_WithSelectedPRN_RedirectsToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache, configurationService, smallProducerSearcher);

            SearchResultsViewModel viewModel = new SearchResultsViewModel()
            {
                SelectedRegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ActionResult result = await controller.SearchResults(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("Details", redirectResult.RouteValues["action"]);
            Assert.Equal("WEE/AA1111AA", redirectResult.RouteValues["RegistrationNumber"]);
        }

        [Fact]
        public async Task GetDetails_FetchesComplianceYearsFromApiAndReturnsViewWithYearValues()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetProducerComplianceYear)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(new List<int> { 2018, 2017, 2016 });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache, configurationService, smallProducerSearcher);

            // Act
            ActionResult result = await controller.Details("WEE/AA1111AA");

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetProducerComplianceYear)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "details");

            DetailsViewModel resultsViewModel = viewResult.Model as DetailsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal(2018, resultsViewModel.SelectedYear);
            Assert.Collection(resultsViewModel.ComplianceYears,
                r1 => Assert.Equal("2018", r1.Text),
                r2 => Assert.Equal("2017", r2.Text),
                r3 => Assert.Equal("2016", r3.Text));
        }

        [Fact]
        public async Task GetDetails_ReturnsDetailsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetProducerComplianceYear)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(new List<int> { 2018, 2017, 2016 });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache, configurationService, smallProducerSearcher);

            // Act
            ActionResult result = await controller.Details("WEE/AA1111AA");

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetProducerComplianceYear)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .MustHaveHappened();

            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "details");

            DetailsViewModel resultsViewModel = viewResult.Model as DetailsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal("WEE/AA1111AA", resultsViewModel.RegistrationNumber);
        }

        [Fact]
        public async Task FetchDetails_FetchesDetailsFromApiAndReturnsDetailsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerComplianceYear>._))
                .WhenArgumentsMatch(a => ((GetProducerComplianceYear)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(new List<int> { 2018, 2017, 2016 });

            ProducerDetails producerDetails = A.Dummy<ProducerDetails>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(producerDetails);

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache, configurationService, smallProducerSearcher);

            // Act
            ActionResult result = await controller.FetchDetails("WEE/AA1111AA", 2015);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .MustHaveHappened();

            PartialViewResult viewResult = result as PartialViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "_detailsresults");

            Assert.Equal(producerDetails, viewResult.Model);
        }

        [Fact]
        public async Task HttpGet_DownloadProducerAmendmentsCsv_ShouldReturnFileContentType()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            CSVFileData csvData = A.Dummy<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerAmendmentsHistoryCSV>._))
                .Returns(new CSVFileData
                {
                    FileName = "test.csv",
                    FileContent = "123,abc"
                });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache, configurationService, smallProducerSearcher);

            //Act
            var result = await controller.DownloadProducerAmendmentsCsv("WEE/AA1111AA");

            //Assert
            Assert.IsType<FileContentResult>(result);
        }

        /// <summary>
        /// This test ensures that the GET "ConfirmRemoval" action returns the "ConfirmRemoval" view
        /// when the current user is allowed to remove producers.
        /// </summary>
        [Fact]
        public async Task GetConfirmRemoval_ReturnsConfirmRemovalView_WhenCanRemoveProducerIsTrue()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                configurationService,
                smallProducerSearcher);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .Returns(
                new ProducerDetailsScheme
                {
                    CanRemoveProducer = true
                });

            // Act
            ActionResult result = await controller.ConfirmRemoval(A.Dummy<Guid>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "confirmremoval");
        }

        /// <summary>
        /// This test ensures that the GET "ConfirmRemoval" action always calls the
        /// API to retrieve details about the specified registered producer which
        /// are used to populate the view model.
        /// </summary>
        [Fact]
        public async Task GetConfirmRemoval_CallsApiForSpecifiedRegisteredProducer_AndPopulatesViewModel()
        {
            // Arrange
            Guid registeredProducerId = new Guid("9F253FE4-B644-4EA1-B58E-19C735512449");

            ProducerDetailsScheme producerDetailsScheme = new ProducerDetailsScheme
            {
                CanRemoveProducer = true
            };

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                    .WhenArgumentsMatch(a => a.Get<GetProducerDetailsByRegisteredProducerId>("request").RegisteredProducerId == registeredProducerId)
                    .Returns(producerDetailsScheme);

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                configurationService, smallProducerSearcher);

            // Act
            ActionResult result = await controller.ConfirmRemoval(registeredProducerId);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            ConfirmRemovalViewModel viewModel = viewResult.Model as ConfirmRemovalViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(producerDetailsScheme, viewModel.Producer);
        }

        /// <summary>
        /// This test ensures that the GET "ConfirmRemoval" action returns the HTTP Forbidden code
        /// when the current user is not allowed to remove producers.
        /// </summary>
        [Fact]
        public async Task GetConfirmRemoval_ReturnsHttpForbiddenResult_WhenCanRemoveProducerIsFalse()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                configurationService,
                smallProducerSearcher);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .Returns(
                new ProducerDetailsScheme
                {
                    CanRemoveProducer = false
                });

            // Act
            var result = await controller.ConfirmRemoval(A.Dummy<Guid>());

            // Assert
            Assert.IsType<HttpForbiddenResult>(result);
        }

        /// <summary>
        /// This test ensures that the POST "ConfirmRemoval" action with an invalid model
        /// returns the "ConfirmRemoval" view.
        /// </summary>
        [Fact]
        public async Task PostConfirmRemoval_WithInvalidModel_ReturnsConfirmRemovalView()
        {
            // Arrange
            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                configurationService, smallProducerSearcher);

            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.ConfirmRemoval(A.Dummy<Guid>(), A.Dummy<ConfirmRemovalViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "confirmremoval");
        }

        /// <summary>
        /// This test ensures that the POST "ConfirmRemoval" action with an invalid model calls the
        /// API to retrieve details about the specified registered producer which
        /// are used to populate the view model.
        /// </summary>
        [Fact]
        public async Task PostConfirmRemoval_WithInvalidModel_CallsApiAndPopulatesViewModel()
        {
            // Arrange
            Guid registeredProducerId = new Guid("9F253FE4-B644-4EA1-B58E-19C735512449");

            ProducerDetailsScheme producerDetailsScheme = A.Dummy<ProducerDetailsScheme>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .WhenArgumentsMatch(a => a.Get<GetProducerDetailsByRegisteredProducerId>("request").RegisteredProducerId == registeredProducerId)
                .Returns(producerDetailsScheme);

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                configurationService,
                smallProducerSearcher);

            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            ActionResult result = await controller.ConfirmRemoval(registeredProducerId, A.Dummy<ConfirmRemovalViewModel>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            ConfirmRemovalViewModel viewModel = viewResult.Model as ConfirmRemovalViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(producerDetailsScheme, viewModel.Producer);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithYesOptionSelected_AndRequestReturnsCacheInvalidation_ShouldInvalidateProducerSearch()
        {
            var viewModel = new ConfirmRemovalViewModel { SelectedValue = "Yes" };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(new RemoveProducerResult(true));

            await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            A.CallTo(() => cache.InvalidateProducerSearch())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithYesOptionSelected_AndRequestDoesNotReturnCacheInvalidation_ShouldNotInvalidateProducerSearch()
        {
            var viewModel = new ConfirmRemovalViewModel { SelectedValue = "Yes" };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(new RemoveProducerResult(false));

            await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            A.CallTo(() => cache.InvalidateProducerSearch())
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HttpPost_ConfirmRemoval_WithYesOptionSelected_SendsRequest_ThenRegardlessOfCacheInvalidation_ShouldRedirectToRemovedAction(bool invalidateCache)
        {
            // Arrange
            var viewModel = new ConfirmRemovalViewModel { SelectedValue = "Yes" };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(new RemoveProducerResult(invalidateCache));

            // Act
            var result = await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            // Assert

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = (((RedirectToRouteResult)result).RouteValues);

            Assert.Equal("Removed", routeValues["action"]);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithNoOptionSelected_DoesNotSendRequest_AndDoesNotInvalidateProducerSearch_ThenRedirectsToDetailsAction()
        {
            // Arrange
            var viewModel = new ConfirmRemovalViewModel { SelectedValue = "No" };

            // Act
            var result = await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .MustNotHaveHappened();

            A.CallTo(() => cache.InvalidateProducerSearch())
                .MustNotHaveHappened();

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Details", routeValues["action"]);
        }

        [Fact]
        public async Task HttpGet_RemovedProducer_ShouldReturnRemovedProducerModel()
        {
            // Act
            var result = await ProducersController().Removed("WEE/MM0001AA", 2016, "SchemeName");

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<RemovedViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public async Task HttpPost_RemovedProducer_ProducerIsAssociatedWithOtherScheme_ShouldRedirectToDetailsAction()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerRegisteredForComplianceYear>._))
                .Returns(true);

            // Act
            var model = new RemovedViewModel
            {
                RegistrationNumber = "ABC12345"
            };

            var result = await ProducersController().Removed(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerRegisteredForComplianceYear>._))
                .MustHaveHappened();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Details", routeValues["action"]);
        }

        [Fact]
        public async Task HttpPost_RemovedProducer_ProducerNotAssociatedWithOtherScheme_ShouldRedirectToSearchAction()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerRegisteredForComplianceYear>._))
                .Returns(false);

            // Act
            var model = new RemovedViewModel
            {
                RegistrationNumber = "ABC12345"
            };

            var result = await ProducersController().Removed(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerRegisteredForComplianceYear>._))
                .MustHaveHappened();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Search", routeValues["action"]);
            Assert.Equal(SearchTypeEnum.Producer, routeValues["searchType"]);
        }

        [Fact]
        public async Task HttpGet_DownloadProducerEeeHistoryCsv_ShouldReturnFileContentType()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            CSVFileData csvData = A.Dummy<CSVFileData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerEeeDataHistoryCsv>._))
                .Returns(new CSVFileData
                {
                    FileName = "test.csv",
                    FileContent = "123,abc"
                });

            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache, configurationService, smallProducerSearcher);

            //Act
            var result = await controller.DownloadProducerEeeDataHistoryCsv("WEE/AA1111AA");

            //Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Theory]
        [InlineData(SearchTypeEnum.Producer, InternalUserActivity.ProducerDetails)]
        [InlineData(SearchTypeEnum.SmallProducer, InternalUserActivity.DirectRegistrantDetails)]
        public async Task GetSearch_ReturnsSearchView_WithCorrectSearchTypeAndBreadcrumb(SearchTypeEnum searchType, string expectedActivity)
        {
            // Act
            var result = await controller.Search(searchType);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<SearchViewModel>();
            var model = viewResult.Model as SearchViewModel;
            model.SearchType.Should().Be(searchType);

            breadcumbService.InternalActivity.Should().Be(expectedActivity);
        }

        [Theory]
        [InlineData(SearchTypeEnum.Producer)]
        [InlineData(SearchTypeEnum.SmallProducer)]
        public async Task GetSearchResults_ReturnsCorrectResultsAndBreadcrumb(SearchTypeEnum searchType)
        {
            // Arrange
            const string searchTerm = "test";
            if (searchType == SearchTypeEnum.Producer)
            {
                A.CallTo(() => producerSearcher.Search(searchTerm, 10, false))
                    .Returns(new List<ProducerSearchResult>() { new ProducerSearchResult() { RegistrationNumber = "WEE/AA1111AA" } });
            }
            else
            {
                A.CallTo(() => smallProducerSearcher.Search(searchTerm, 10, false))
                    .Returns(new List<SmallProducerSearchResult>() { new SmallProducerSearchResult() { RegistrationNumber = "WEE/BB2222BB" } });
            }

            // Act
            var result = await controller.SearchResults(searchTerm, searchType);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<SearchResultsViewModel>();
            var model = viewResult.Model as SearchResultsViewModel;

            model.SearchType.Should().Be(searchType);
            model.Results.Should().Contain(a =>
                a.RegistrationNumber == (searchType == SearchTypeEnum.SmallProducer ? "WEE/BB2222BB" : "WEE/AA1111AA"));

            var expectedActivity = searchType == SearchTypeEnum.Producer ? InternalUserActivity.ProducerDetails : InternalUserActivity.DirectRegistrantDetails;
            breadcumbService.InternalActivity.Should().Be(expectedActivity);
        }

        [Theory]
        [InlineData(SearchTypeEnum.SmallProducer, "ProducerSubmission", "Submissions")]
        [InlineData(SearchTypeEnum.Producer, "Producers", "Details")]
        public async Task PostSearchResults_RedirectsToCorrectAction(SearchTypeEnum searchType, string expectedController, string expectedAction)
        {
            // Arrange
            var viewModel = new SearchResultsViewModel { SearchType = searchType, SelectedRegistrationNumber = "WEE/AA1111AA" };

            // Act
            var result = await controller.SearchResults(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.RouteValues["action"].Should().Be(expectedAction);
            redirectResult.RouteValues["controller"].Should().Be(expectedController);
            redirectResult.RouteValues["RegistrationNumber"].Should().Be("WEE/AA1111AA");

            var expectedActivity = searchType == SearchTypeEnum.Producer ? InternalUserActivity.ProducerDetails : InternalUserActivity.DirectRegistrantDetails;
            breadcumbService.InternalActivity.Should().Be(expectedActivity);
        }

        private ProducersController ProducersController()
        {
            return new ProducersController(breadcumbService, producerSearcher, () => weeeClient, cache, configurationService, smallProducerSearcher);
        }
    }
}
