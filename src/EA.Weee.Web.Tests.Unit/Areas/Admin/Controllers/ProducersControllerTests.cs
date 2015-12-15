namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using Xunit;

    public class ProducersControllerTests
    {
        private readonly BreadcrumbService breadcumbService;
        private readonly ISearcher<ProducerSearchResult> producerSearcher;
        private readonly IWeeeClient weeeClient;

        public ProducersControllerTests()
        {
            breadcumbService = A.Fake<BreadcrumbService>();
            producerSearcher = A.Fake<ISearcher<ProducerSearchResult>>();
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();
            Func<IWeeeClient> weeeClient = A.Dummy<Func<IWeeeClient>>();

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClient);

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
        public async Task GetDetails_FetchesDetailsFromApiAndReturnsDetailsView()
        {
            // Arrange
            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();
            ISearcher<ProducerSearchResult> producerSearcher = A.Dummy<ISearcher<ProducerSearchResult>>();

            ProducerDetails producerDetails = A.Dummy<ProducerDetails>();

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .Returns(producerDetails);
            
            Func<IWeeeClient> weeeClientFunc = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => weeeClientFunc())
                .Returns(weeeClient);

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            // Act
            ActionResult result = await controller.Details("WEE/AA1111AA");

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetails>._))
                .WhenArgumentsMatch(a => ((GetProducerDetails)a[1]).RegistrationNumber == "WEE/AA1111AA")
                .MustHaveHappened();
            
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName.ToLowerInvariant() == "details");

            DetailsViewModel resultsViewModel = viewResult.Model as DetailsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Equal(producerDetails, resultsViewModel.Details);
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

            ProducersController controller = new ProducersController(breadcrumb, producerSearcher, weeeClientFunc);

            //Act
            var result = await controller.DownloadProducerAmendmentsCsv("WEE/AA1111AA");

            //Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void HttpGet_ConfirmRemoval_ShouldSendRequest_AndShouldReturnConfirmRemovalModel()
        {
            // Arrange
            var producerDetailsScheme = A.Dummy<ProducerDetailsScheme>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .Returns(producerDetailsScheme);

            var registeredProducerId = Guid.NewGuid();

            // Act
            ActionResult result = await ProducersController().ConfirmRemoval(registeredProducerId);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerDetailsByRegisteredProducerId>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ConfirmRemovalViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithInvalidModel_ReturnsConfirmRemovalModel()
        {
            // Arrange
            var controller = ProducersController();
            var viewModel = new ConfirmRemovalViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            var result = await controller.ConfirmRemoval(viewModel);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ConfirmRemovalViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithYesOptionSelected_SendsRequest_ThenRedirectsToRemovedAction()
        {
            // Arrange
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "Yes";

            // Act
            var result = await ProducersController().ConfirmRemoval(viewModel);

            // Assert

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = (((RedirectToRouteResult)result).RouteValues);

            Assert.Equal("Removed", routeValues["action"]);
        }

        [Fact]
        public async Task HttpPost_ConfirmRemoval_WithNoOptionSelected_DoesNotSendRequest_ThenRedirectsToDetailsAction()
        {
            // Arrange
            var viewModel = new ConfirmRemovalViewModel();
            viewModel.SelectedValue = "No";

            // Act
            var result = await ProducersController().ConfirmRemoval(viewModel);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<RemoveProducer>._))
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
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociatedWithAnotherScheme>._))
                .Returns(true);

            // Act
            var model = new RemovedViewModel
            {
                RegistrationNumber = "ABC12345"
            };

            var result = await ProducersController().Removed(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociatedWithAnotherScheme>._))
                .MustHaveHappened();

            Assert.IsType<RedirectToRouteResult>(result);
            
            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Details", routeValues["action"]);
        }

        [Fact]
        public async void HttpPost_RemovedProducer_ProducerNotAssociatedWithOtherScheme_ShouldRedirectToSearchAction()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociatedWithAnotherScheme>._))
                .Returns(false);

            // Act
            var model = new RemovedViewModel
            {
                RegistrationNumber = "ABC12345"
            };

            var result = await ProducersController().Removed(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsProducerAssociatedWithAnotherScheme>._))
                .MustHaveHappened();

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Search", routeValues["action"]);
        }

        private ProducersController ProducersController()
        {
            return new ProducersController(breadcumbService, producerSearcher, () => weeeClient);
        }
    }
}
