namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using Infrastructure;
    using Services.Caching;
    using Xunit;

    public class ProducersControllerTests
    {
        private readonly BreadcrumbService breadcumbService;
        private readonly ISearcher<ProducerSearchResult> producerSearcher;
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache cache;

        public ProducersControllerTests()
        {
            breadcumbService = A.Fake<BreadcrumbService>();
            producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            weeeClient = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
        }

        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

            // Act
            ActionResult result = await controller.Search();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithInvalidModel_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm";
            viewModel.SelectedRegistrationNumber = null;

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Fact]
        public async Task PostSearch_WithSelectedPRN_RedirectsToDetailsAction()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

            SearchViewModel viewModel = new SearchViewModel();
            viewModel.SearchTerm = "testSearchTerm, WEE/AA1111AA";
            viewModel.SelectedRegistrationNumber = "WEE/AA1111AA";

            // Act
            ActionResult result = await controller.Search(viewModel);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("Details", redirectResult.RouteValues["action"]);
            Assert.Equal("WEE/AA1111AA", redirectResult.RouteValues["RegistrationNumber"]);
        }

        [Fact]
        public async Task GetSearchResults_DoesSearchForTenResultsAndReturnsSearchReturnsView()
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

            // Act
            ActionResult result = await controller.SearchResults("testSearchTerm");

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "searchresults");

            SearchResultsViewModel viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Contains(viewModel.Results, r => r.RegistrationNumber == "WEE/AA1111AA");
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = "testSearchTerm";
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient, cache);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache);

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
        public async void HttpGet_DownloadProducerAmendmentsCsv_ShouldReturnFileContentType()
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache);

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
        public async void GetConfirmRemoval_ReturnsConfirmRemovalView_WhenCanRemoveProducerIsTrue()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>());

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
        public async void GetConfirmRemoval_CallsApiForSpecifiedRegisteredProducer_AndPopulatesViewModel()
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
                A.Dummy<IWeeeCache>());

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
        public async void GetConfirmRemoval_ReturnsHttpForbiddenResult_WhenCanRemoveProducerIsFalse()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            ProducersController controller = new ProducersController(
                A.Dummy<BreadcrumbService>(),
                A.Dummy<ISearcher<ProducerSearchResult>>(),
                () => weeeClient,
                A.Dummy<IWeeeCache>());

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
                A.Dummy<IWeeeCache>());

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
                A.Dummy<IWeeeCache>());

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
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "Yes";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(new RemoveProducerResult(true));

            await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            A.CallTo(() => cache.InvalidateProducerSearch())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithYesOptionSelected_AndRequestDoesNotReturnCacheInvalidation_ShouldNotInvalidateProducerSearch()
        {
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "Yes";

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
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "Yes";

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .Returns(new RemoveProducerResult(invalidateCache));

            // Act
            var result = await ProducersController().ConfirmRemoval(A.Dummy<Guid>(), viewModel);

            // Assert

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = (((RedirectToRouteResult)result).RouteValues);

            Assert.Equal("Removed", routeValues["action"]);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithNoOptionSelected_DoesNotSendRequest_AndDoesNotInvalidateProducerSearch_ThenRedirectsToDetailsAction()
        {
            // Arrange
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "No";

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
        public async void HttpGet_RemovedProducer_ShouldReturnRemovedProducerModel()
        {
            // Act
            var result = await ProducersController().Removed("WEE/MM0001AA", 2016, "SchemeName");

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<RemovedViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public async void HttpPost_RemovedProducer_ProducerIsAssociatedWithOtherScheme_ShouldRedirectToDetailsAction()
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
        public async void HttpPost_RemovedProducer_ProducerNotAssociatedWithOtherScheme_ShouldRedirectToSearchAction()
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
        }

        [Fact]
        public async void HttpGet_DownloadProducerEeeHistoryCsv_ShouldReturnFileContentType()
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc, cache);

            //Act
            var result = await controller.DownloadProducerEeeDataHistoryCsv("WEE/AA1111AA");

            //Assert
            Assert.IsType<FileContentResult>(result);
        }

        private ProducersController ProducersController()
        {
            return new ProducersController(breadcumbService, producerSearcher, () => weeeClient, cache);
        }
    }
}
