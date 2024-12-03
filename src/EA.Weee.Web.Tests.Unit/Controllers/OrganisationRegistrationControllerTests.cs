﻿namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Core.Organisations;
    using Core.Shared;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Search;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Controllers;
    using Web.ViewModels.OrganisationRegistration;
    using Weee.Requests.Organisations;
    using Xunit;
    using OrganisationData = Core.Organisations.OrganisationData;

    public class OrganisationRegistrationControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly ISearcher<OrganisationSearchResult> organisationSearcher;
        private readonly ConfigurationService configurationService;
        private readonly IOrganisationTransactionService transactionService;
        private readonly OrganisationRegistrationController controller;
        private readonly IWeeeCache weeeCache;
        private readonly ICompaniesHouseClient companiesHouseClient;
        private readonly IAddressLookupClient addressLookupClient;

        public OrganisationRegistrationControllerTests()
        {
            configurationService = A.Fake<ConfigurationService>();
            transactionService = A.Fake<IOrganisationTransactionService>();
            weeeClient = A.Fake<IWeeeClient>();
            organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            weeeCache = A.Fake<IWeeeCache>();
            companiesHouseClient = A.Fake<ICompaniesHouseClient>();
            addressLookupClient = A.Fake<IAddressLookupClient>();

            controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService,
                transactionService,
                weeeCache,
                () => companiesHouseClient,
                () => addressLookupClient);

            A.CallTo(() => configurationService.CurrentConfiguration.MaximumOrganisationSearchResults).Returns(5);
        }

        [Fact]
        public async Task GetJoinOrganisationConfirmation_ReturnsJoinOrganisationConfirmationView()
        {
            // Arrange
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test"
            };

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService,
                transactionService,
                weeeCache,
                () => companiesHouseClient,
                () => addressLookupClient);

            // Act
            ActionResult result = await controller.JoinOrganisationConfirmation(orgData.Id, true);

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationConfirmationViewModel>(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetJoinOrganisationConfirmation_GivenActiveUsers_ActiveUsersIsSetInViewModel(bool activeUsers)
        {
            // Arrange
            var orgData = new PublicOrganisationData
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(orgData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            // Act
            ActionResult result = await controller.JoinOrganisationConfirmation(orgData.Id, activeUsers);

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationConfirmationViewModel viewModel = model as JoinOrganisationConfirmationViewModel;

            viewModel.AnyActiveUsers.Should().Be(activeUsers);
        }

        [Fact]
        public async Task GetJoinOrganisation_ReturnsView()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<JoinOrganisationViewModel>(model);
        }

        [Fact]
        public async Task GetJoinOrganisation_GivenActiveUsers_ReturnsViewWithActiveUsersSet()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var activeUsers = new List<OrganisationUserData>()
            {
                A.Fake<OrganisationUserData>()
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationViewModel viewModel = model as JoinOrganisationViewModel;

            viewModel.AnyActiveUsers.Should().Be(true);
        }

        [Fact]
        public async Task GetJoinOrganisation_GivenNoActiveUsers_ReturnsViewWithActiveUsersSet()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(new PublicOrganisationData());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var activeUsers = new List<OrganisationUserData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            // Act
            ActionResult result = await controller.JoinOrganisation(A.Dummy<Guid>());

            // Assert
            var model = ((ViewResult)result).Model;

            JoinOrganisationViewModel viewModel = model as JoinOrganisationViewModel;

            viewModel.AnyActiveUsers.Should().Be(false);
        }

        [Fact]
        public async Task GetJoinOrganisation_UserAlreadyAssociated_ReturnsUserAlreadyAssociatedWithOrgansiationView()
        {
            // Arrange
            var organisationId = new Guid("101F5E58-FEA3-4F59-9281-E543EDE5699F");

            var organisation = new PublicOrganisationData()
            {
                Id = organisationId,
                DisplayName = "Test Company"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(organisation);

            var association = new OrganisationUserData()
            {
                OrganisationId = organisationId,
                UserStatus = UserStatus.Active,
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>() { association });

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var activeUsers = new List<OrganisationUserData>()
            {
                A.Fake<OrganisationUserData>()
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetActiveOrganisationUsers>._)).Returns(activeUsers);

            // Act
            ActionResult result = await controller.JoinOrganisation(organisationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.Equal("UserAlreadyAssociatedWithOrganisation", viewResult.ViewName);

            var viewModel = viewResult.Model as UserAlreadyAssociatedWithOrganisationViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(organisationId, viewModel.OrganisationId);
            Assert.Equal(UserStatus.Active, viewModel.Status);
            Assert.Equal("Test Company", viewModel.OrganisationName);
            Assert.True(viewModel.AnyActiveUsers);
        }

        [Fact]
        public async Task PostJoinOrganisation_NoSearchAnotherOrganisationSelected_RedirectsToType()
        {
            // Arrange
            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var model = new JoinOrganisationViewModel { SelectedValue = "No" };

            // Act
            var result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("Search", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostJoinOrganisation_YesJoinOrganisationSelected_RedirectsToJoinOrganisationConfirmation()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<JoinOrganisation>._))
                .Returns(Guid.NewGuid());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            var model = new JoinOrganisationViewModel { SelectedValue = "Yes - join xyz" };

            // Act
            var result = await controller.JoinOrganisation(model);

            // Assert
            var redirectToRouteResult = ((RedirectToRouteResult)result);

            Assert.Equal("JoinOrganisationConfirmation", redirectToRouteResult.RouteValues["action"]);
        }

        [Fact]
        public async Task GetSearch_UserHasNotOrganisation_ShowPerformAnotherActivityLinkIsFalse()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>());

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            // Act
            var result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.False(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task GetSearch_UserHasOrganisations_ShowPerformAnotherActivityLinkIsTrue()
        {
            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData(),
                    new OrganisationUserData()
                });

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            // Act
            var result = await controller.Search();

            // Assert
            var model = ((ViewResult)result).Model as SearchViewModel;

            Assert.NotNull(model);
            Assert.True(model.ShowPerformAnotherActivityLink);
        }

        [Fact]
        public async Task GetSearch_ReturnsSearchView()
        {
            // Arrange
            // Act
            var result = await controller.Search();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithInvalidModel_ReturnsSearchView()
        {
            // Arrange
            var viewModel = new SearchViewModel();
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "search");
        }

        [Fact]
        public async Task PostSearch_WithSearchTermAndNoSelectedOrganisationId_RedirectsToSearchResultsAction()
        {
            // Arrange
            var viewModel = new SearchViewModel { SearchTerm = "testSearchTerm", SelectedOrganisationId = null };

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("SearchResults", redirectResult.RouteValues["action"]);
            Assert.Equal("testSearchTerm", redirectResult.RouteValues["SearchTerm"]);
        }

        [Fact]
        public async Task PostSearch_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            var viewModel = new SearchViewModel
            {
                SearchTerm = "Test Company",
                SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8")
            };

            // Act
            var result = await controller.Search(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(redirectResult.RouteValues["OrganisationId"],
                new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task GetSearchResults_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            // Arrange
            var fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            var organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService,
                weeeCache,
                () => companiesHouseClient,
                () => addressLookupClient);

            // Act
            var result = await controller.SearchResults("testSearchTerm");

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "searchresults");

            var viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Contains(viewModel.Results,
                r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithInvalidModel_DoesSearchForFiveResultsAndReturnsSearchReturnsView()
        {
            var fakeResults = new List<OrganisationSearchResult>()
            {
                new OrganisationSearchResult()
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company",
                }
            };

            var organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            var weeeClient = A.Dummy<Func<IWeeeClient>>();

            var controller = new OrganisationRegistrationController(
                weeeClient,
                organisationSearcher,
                configurationService,
                transactionService,
                weeeCache,
                () => companiesHouseClient,
                () => addressLookupClient);

            var viewModel = new SearchResultsViewModel { SearchTerm = "testSearchTerm" };
            controller.ModelState.AddModelError("SomeProperty", "Exception");

            // Act
            var result = await controller.SearchResults(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) ||
                        viewResult.ViewName.ToLowerInvariant() == "searchresults");

            var resultsViewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(resultsViewModel);

            Assert.Contains(resultsViewModel.Results,
                r => r.OrganisationId == new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task PostSearchResults_WithSelectedOrganisationId_RedirectsToJoinOrganisationAction()
        {
            // Arrange
            var viewModel = new SearchResultsViewModel()
            {
                SelectedOrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8")
            };

            // Act
            var result = await controller.SearchResults(viewModel);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("JoinOrganisation", redirectResult.RouteValues["action"]);
            Assert.Equal(redirectResult.RouteValues["OrganisationId"],
                new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"));
        }

        [Fact]
        public async Task TypeGet_ReturnsViewWithViewModel_WithSearchText()
        {
            var result = await controller.Type() as ViewResult;

            var resultViewModel = result.Model as OrganisationTypeViewModel;

            resultViewModel.Should().NotBeNull();

            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Type");
        }

        [Fact]
        public async Task Type_Get_RetrievesTransactionDataAndPopulatesViewModel()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                OrganisationType = TestFixture.Create<ExternalOrganisationType>()
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.Type() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrWhiteSpace();

            var model = result.Model as OrganisationTypeViewModel;
            model.Should().NotBeNull();
            model.SelectedValue.Should().Be(organisationTransactionData.OrganisationType.GetDisplayName());
        }

        [Theory]
        [InlineData("Sole trader", "SoleTraderDetails")]
        [InlineData("Partnership", "PartnerDetails")]
        [InlineData("Registered company", "OrganisationDetails")]
        public async Task TypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action)
        {
            var viewModel = new OrganisationTypeViewModel()
            {
                SelectedValue = selectedValue,
            };

            var result = await controller.Type(viewModel) as RedirectToRouteResult;

            A.CallTo(() => transactionService.CaptureData(A<string>._, viewModel)).MustHaveHappenedOnceExactly();

            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["organisationType"].Should().Be(viewModel.SelectedValue);
        }

        [Fact]
        public async void TonnageTypeGet_ReturnsViewWithViewModel_WithSearchText()
        {
            // Arrange
            const string searchText = "company";

            // Act
            var result = await controller.TonnageType(searchText);

            // Assert
            var resultViewModel = result.Model as TonnageTypeViewModel;
            Assert.NotNull(result);
            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Tonnage");
        }

        [Fact]
        public async Task TonnageType_Get_RetrievesTransactionDataAndPopulatesViewModel()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                TonnageType = TonnageType.FiveTonnesOrMore,
                SearchTerm = "Test Company",
                NpwdMigrated = false
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);
            // Act
            var result = await controller.TonnageType(organisationTransactionData.SearchTerm);

            // Assert
            var resultViewModel = result.Model as TonnageTypeViewModel;
            Assert.NotNull(result);
            Assert.Equal(organisationTransactionData.SearchTerm, resultViewModel.SearchedText);
            Assert.Equal("5 tonnes or more", resultViewModel.SelectedValue);
            Assert.Equal(organisationTransactionData.NpwdMigrated, resultViewModel.NpwdMigrated);
        }

        [Fact]
        public async void TonnageTypePost_ModelNotValid_ReturnsView()
        {
            // Arrange
            controller.ModelState.AddModelError("error", "error");

            const string searchText = "company";
            var viewModel = new TonnageTypeViewModel()
            {
                SearchedText = searchText
            };

            // Act
            var result = await controller.TonnageType(viewModel) as ViewResult;

            // Assert
            var resultViewModel = result.Model as TonnageTypeViewModel;
            Assert.NotNull(result);
            Assert.Equal(searchText, resultViewModel.SearchedText);
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "Tonnage");
        }

        [Theory]
        [InlineData("5 tonnes or more", "FiveTonnesOrMore", "OrganisationRegistration")]
        [InlineData("Less than 5 tonnes", "PreviousRegistration", "OrganisationRegistration")]
        public async Task TonnageTypePost_ValidViewModel_ReturnsCorrectRedirect(string selectedValue, string action,
            string correctController)
        {
            // Arrange
            const string searchText = "company";
            var viewModel = new TonnageTypeViewModel()
            {
                SearchedText = searchText,
                SelectedValue = selectedValue
            };

            // Act
            var result = await controller.TonnageType(viewModel) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be(correctController);

            A.CallTo(() =>
                    transactionService.CaptureData(A<string>._, A<TonnageTypeViewModel>.That.IsSameAs(viewModel)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TonnageTypePost_ValidViewModel_ReturnsCorrectRedirect_WhenNpwdMigratedIsTrue()
        {
            // Arrange
            const string searchText = "company";
            var viewModel = new TonnageTypeViewModel()
            {
                SearchedText = searchText,
                SelectedValue = "Less than 5 tonnes",
                NpwdMigrated = true
            };

            // Act
            var result = await controller.TonnageType(viewModel) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            result.RouteValues["action"].Should().Be("AuthorisedRepresentative");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public void FiveTonnesOrMoreGet_ReturnsView()
        {
            // Arrange
            // Act
            var result = controller.FiveTonnesOrMore();

            // Assert
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "FiveTonnesOrMore");
        }

        [Theory]
        [InlineData("Yes, I have previously been a member of a producer compliance scheme", "AuthorisedRepresentative")]
        [InlineData("Yes, I have previously been registered directly as a small producer", "Search")]
        [InlineData("No", "AuthorisedRepresentative")]
        public async Task PreviousRegistration_Post_WhenModelValid_CapturesDataAndRedirectsToType(string selectedValue,
            string action)
        {
            // Arrange
            var model = new PreviousRegistrationViewModel { SelectedValue = selectedValue };

            // Act
            var result = await controller.PreviousRegistration(model) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.CaptureData(A<string>._, model))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(action);
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task PreviousRegistration_Get_RetrievesTransactionDataAndPopulatesViewModel()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                PreviousRegistration = PreviouslyRegisteredProducerType.YesPreviousSmallProducer,
                SearchTerm = "Test Company"
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.PreviousRegistration() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrWhiteSpace();
            var model = result.Model as PreviousRegistrationViewModel;
            model.Should().NotBeNull();
            model.SelectedValue.Should().Be("Yes, I have previously been registered directly as a small producer");
        }

        [Fact]
        public async Task RegisterSmallProducer_ClearsTransactionDataAndRedirectsToTonnageType()
        {
            // Arrange
            const string searchTerm = "Test Company";

            // Act
            var result = await controller.RegisterSmallProducer(searchTerm) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.DeleteOrganisationTransactionData(A<string>._))
                .MustHaveHappenedOnceExactly();

            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TonnageType");
            result.RouteValues["searchTerm"].Should().Be(searchTerm);
            result.RouteValues["controller"].Should().BeNull();
        }

        [Theory]
        [InlineData(YesNoType.No, "RegistrationComplete", "OrganisationRegistration")]
        [InlineData(YesNoType.Yes, "RepresentingCompanyDetails", "OrganisationRegistration")]
        public async Task OrganisationDetails_Post_ValidModel_RedirectsToHoldingController(YesNoType authorisedRep,
            string index, string controllerName)
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = null;
            model.IsPreviousSchemeMember = false;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, authorisedRep).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).Returns(organisationId);

            A.CallTo(() => this.weeeClient
                    .SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                .Returns(Task.FromResult(new List<EA.Weee.Core.Organisations.OrganisationData>()));

            A.CallTo(() => organisationSearcher.Search(A<string>._, A<int>._, A<bool>._))
                .Returns(new List<OrganisationSearchResult>());

            // Act
            var result = await controller.OrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(index);
            result.RouteValues["controller"].Should().Be(controllerName);
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .MustHaveHappenedOnceExactly();

            if (authorisedRep == YesNoType.No)
            {
                A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustHaveHappenedOnceExactly();
                A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
                result.RouteValues["organisationId"].Should().Be(organisationId);
            }
            else
            {
                A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustNotHaveHappened();
                A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustNotHaveHappened();
            }
        }

        [Fact]
        public async Task OrganisationDetails_Post_ValidModel_ChecksOrganisationExistence()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = null;
            model.IsPreviousSchemeMember = false;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.Yes).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).Returns(organisationId);

            // Act
            await controller.OrganisationDetails(model);

            // Assert
            A.CallTo(() => this.weeeClient
                    .SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>
                        .That
                        .Matches(w => w.RegistrationNumber == model.CompaniesRegistrationNumber)))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task OrganisationDetails_Post_ValidModel_ChecksOrganisationExistenceWithNameSearchIfRegReturnsEmptyList()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = null;
            model.IsPreviousSchemeMember = false;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.Yes).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).Returns(organisationId);

            A.CallTo(() => this.weeeClient
                    .SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>
                        .That
                        .Matches(w => w.RegistrationNumber == model.CompaniesRegistrationNumber)))
                .Returns(Task.FromResult(new List<EA.Weee.Core.Organisations.OrganisationData>()));

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            A.CallTo(() => this.organisationSearcher
                    .Search(model.CompanyName, A<int>._, A<bool>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task OrganisationDetails_Post_ValidModel_RedirectsIfOrgExists()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = null;
            model.IsPreviousSchemeMember = false;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.Yes).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).Returns(organisationId);

            var org = new EA.Weee.Core.Organisations.OrganisationData { Id = Guid.NewGuid() };

            A.CallTo(() => this.weeeClient
                    .SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>
                        .That
                        .Matches(w => w.RegistrationNumber == model.CompaniesRegistrationNumber)))
                .Returns(new List<EA.Weee.Core.Organisations.OrganisationData> { org });

            // Act
            var result = await controller.OrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.RouteValues["action"].Should().Be("OrganisationFound");
        }

        public static IEnumerable<object[]> OrganisationTypeData()
        {
            yield return new object[] { ExternalOrganisationType.RegisteredCompany, typeof(RegisteredCompanyDetailsViewModel) };
            yield return new object[] { ExternalOrganisationType.Partnership, typeof(PartnershipDetailsViewModel) };
            yield return new object[] { ExternalOrganisationType.SoleTrader, typeof(SoleTraderDetailsViewModel) };
        }

        [Fact]
        public async Task OrganisationDetails_Get_ReturnsViewWithPopulatedViewModel()
        {
            // Arrange
            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            var existingTransaction = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.RegisteredCompany,
                SearchTerm = null, 
                PreviousRegistration = PreviouslyRegisteredProducerType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            var resultViewModel = result.Model as RegisteredCompanyDetailsViewModel;

            resultViewModel.Should().NotBeNull();
            resultViewModel.CompanyName.Should().BeNullOrWhiteSpace();
            resultViewModel.Address.Countries.Should().BeEquivalentTo(countries);
        }

        [Theory]
        [MemberData(nameof(OrganisationTypeData))]
        public async Task OrganisationDetails_Get_ReturnsCorrectViewModelForOrganisationType(
            ExternalOrganisationType organisationType, Type expectedViewModelType)
        {
            // Arrange
            var countries = SetupCountries();
            var existingTransaction = new OrganisationTransactionData
            {
                OrganisationType = organisationType,
                SearchTerm = TestFixture.Create<string>(),
                PreviousRegistration = PreviouslyRegisteredProducerType.YesPreviousSchemeMember,
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrWhiteSpace();

            result.Model.Should().BeOfType(expectedViewModelType);

            var resultViewModel = result.Model as OrganisationViewModel;
            resultViewModel.Should().NotBeNull();
            resultViewModel.CompanyName.Should().Be(existingTransaction.SearchTerm);
            resultViewModel.Address.Countries.Should().BeEquivalentTo(countries);
            resultViewModel.OrganisationType.Should().Be(organisationType);
        }

        [Theory]
        [InlineData(PreviouslyRegisteredProducerType.YesPreviousSchemeMember, true)]
        [InlineData(PreviouslyRegisteredProducerType.YesPreviousSmallProducer, false)]
        [InlineData(PreviouslyRegisteredProducerType.No, false)]
        public async Task OrganisationDetails_Get_ReturnsFalseSchemeMemberWhenPreviousRegistrationIsFalse(PreviouslyRegisteredProducerType previousRegistration, bool value)
        {
            // Arrange
            var countries = SetupCountries();
            var existingTransaction = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.RegisteredCompany,
                SearchTerm = TestFixture.Create<string>(),
                PreviousRegistration = previousRegistration
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrWhiteSpace();

            var resultViewModel = result.Model as OrganisationViewModel;
            resultViewModel.Should().NotBeNull();
            resultViewModel.IsPreviousSchemeMember.Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(OrganisationTypeData))]
        public async Task OrganisationDetails_Post_InValidModel_ReturnsView(
            ExternalOrganisationType organisationType, Type expectedViewModelType)
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>()
                .With(m => m.OrganisationType, organisationType)
                .Without(c => c.CompanyName)
                .Create();

            controller.ModelState.AddModelError("error", "error");

            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };
            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();

            result.Model.Should().BeOfType(expectedViewModelType);

            var resultModel = result.Model as OrganisationViewModel;
            resultModel.Should().NotBeNull();
            resultModel.Should().BeEquivalentTo(model, options => options
                .Excluding(m => m.Address.Countries));
            resultModel.Address.Countries.Should().BeEquivalentTo(countries);
            resultModel.OrganisationType.Should().Be(organisationType);

            A.CallTo(() => transactionService.CaptureData(A<string>._, A<OrganisationViewModel>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task OrganisationDetails_Post_ReturnsCorrectDetailsForInvalidCompaniesHouseApiCall()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.Action = "Find Company";

            var countries = new List<CountryData>
            {
                new CountryData { Id = UkCountry.Ids.England, Name = "United Kingdom" }
            };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            DefraCompaniesHouseApiModel defraCompaniesHouseApiModel = new DefraCompaniesHouseApiModel()
            {
                Error = true
            };

            A.CallTo(() =>
                    companiesHouseClient.GetCompanyDetailsAsync(
                        configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                        model.CompaniesRegistrationNumber))
                .Returns(defraCompaniesHouseApiModel);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            var resultViewModel = result.Model as OrganisationViewModel;
            resultViewModel.LookupFound.Should().BeFalse();
            resultViewModel.Should().NotBeNull();
            resultViewModel.Address.Countries.Should().BeEquivalentTo(countries);
            A.CallTo(() =>
                companiesHouseClient.GetCompanyDetailsAsync(
                    configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                    model.CompaniesRegistrationNumber)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationDetails_Post_ReturnsCorrectDetailsForValidCompaniesHouseApiCall()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.Action = "Find Company";

            var countries = new List<CountryData>
            {
                new CountryData { Id = UkCountry.Ids.England, Name = "United Kingdom" }
            };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            var defraCompaniesHouseApiModel = TestFixture.Build<DefraCompaniesHouseApiModel>()
                .With(d => d.Error, false)
                .With(d => d.InvalidReference, false)
                .Create();
            defraCompaniesHouseApiModel.Organisation.RegisteredOffice.Country.Name = countries[0].Name;

            A.CallTo(() =>
                    companiesHouseClient.GetCompanyDetailsAsync(
                        configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                        model.CompaniesRegistrationNumber))
                .Returns(defraCompaniesHouseApiModel);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            var resultViewModel = result.Model as OrganisationViewModel;
            var countryName =
                UkCountry.GetIdByName(defraCompaniesHouseApiModel.Organisation.RegisteredOffice.Country.Name);
            resultViewModel.Should().NotBeNull();
            resultViewModel.LookupFound.Should().BeTrue();
            resultViewModel.CompanyName.Should().Be(defraCompaniesHouseApiModel.Organisation.Name);
            resultViewModel.CompaniesRegistrationNumber.Should()
                .Be(defraCompaniesHouseApiModel.Organisation.RegistrationNumber);
            resultViewModel.Address.Address1.Should()
                .Be(defraCompaniesHouseApiModel.Organisation.RegisteredOffice.BuildingNumber);
            resultViewModel.Address.Address2.Should()
                .Be(defraCompaniesHouseApiModel.Organisation.RegisteredOffice.Street);
            resultViewModel.Address.TownOrCity.Should()
                .Be(defraCompaniesHouseApiModel.Organisation.RegisteredOffice.Town);
            resultViewModel.Address.Postcode.Should()
                .Be(defraCompaniesHouseApiModel.Organisation.RegisteredOffice.Postcode);
            resultViewModel.Address.CountryId.Should().Be(countryName);
            A.CallTo(() =>
                companiesHouseClient.GetCompanyDetailsAsync(
                    configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                    model.CompaniesRegistrationNumber)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RepresentingCompanyDetails_Get_ReturnsViewWithPopulatedViewModel()
        {
            // Arrange
            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };
            string returnUrl = "/organisation-found";

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            // Act
            var result = await controller.RepresentingCompanyDetails(returnUrl) as ViewResult;

            // Assert
            var resultViewModel = result.Model as RepresentingCompanyDetailsViewModel;

            Assert.Equal(result.ViewBag.ReturnUrl, returnUrl);

            resultViewModel.Should().NotBeNull();
            resultViewModel.CompanyName.Should().BeNullOrWhiteSpace();
            resultViewModel.Address.Countries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public async Task RepresentingCompanyDetails_Post_ValidModel_RedirectsToRegistrationComplete()
        {
            // Arrange
            var model = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).Returns(organisationId);

            // Act
            var result = await controller.RepresentingCompanyDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RegistrationComplete");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["organisationId"].Should().Be(organisationId);
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RepresentingCompanyDetails_Post_InValidModel_ReturnsView()
        {
            // Arrange
            var model = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            controller.ModelState.AddModelError("error", "error");

            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            // Act
            var result = await controller.RepresentingCompanyDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();

            var resultModel = result.Model as RepresentingCompanyDetailsViewModel;
            resultModel.Should().BeEquivalentTo(model);

            model.Address.Countries.Should().BeEquivalentTo(countries);
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustNotHaveHappened();
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustNotHaveHappened();
            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustNotHaveHappened();
        }

        [Fact]
        public async Task OrganisationDetails_Get_WithExistingTransaction_ReturnsViewWithPopulatedViewModel()
        {
            // Arrange
            var countries = SetupCountries();

            var existingTransaction = new OrganisationTransactionData
            {
                OrganisationViewModel = new OrganisationViewModel
                {
                    CompanyName = "Existing Company",
                    CompaniesRegistrationNumber = "12345678",
                },

                PreviousRegistration = PreviouslyRegisteredProducerType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as RegisteredCompanyDetailsViewModel;
            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(existingTransaction.OrganisationViewModel);
            model.Address.Countries.Should().BeEquivalentTo(countries);
        }

        private List<CountryData> SetupCountries()
        {
            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            return countries;
        }

        [Fact]
        public async Task AuthorisedRepresentative_Get_RetrievesTransactionDataAndPopulatesViewModel()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                AuthorisedRepresentative = YesNoType.Yes,
                SearchTerm = "Test Company"
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.AuthorisedRepresentative() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrWhiteSpace();

            var model = result.Model as AuthorisedRepresentativeViewModel;
            model.Should().NotBeNull();
            model.SelectedValue.Should().Be("Yes");
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task AuthorisedRepresentative_Post_WhenModelValid_CapturesDataAndRedirectsToOrganisationType(
            string selectedValue)
        {
            // Arrange
            var model = new AuthorisedRepresentativeViewModel { SelectedValue = selectedValue };

            // Act
            var result = await controller.AuthorisedRepresentative(model) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.CaptureData(A<string>._, model))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("ContactDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Theory]
        [InlineData(ExternalOrganisationType.RegisteredCompany, "OrganisationDetails")]
        [InlineData(ExternalOrganisationType.Partnership, "OrganisationDetails")]
        [InlineData(ExternalOrganisationType.SoleTrader, "OrganisationDetails")]
        public async Task RepresentingCompanyRedirect_ShouldRedirectToCorrectAction_WhenOrganisationTypeIsSet(
            ExternalOrganisationType organisationType, string expectedAction)
        {
            // Arrange
            var transactionData = new OrganisationTransactionData { OrganisationType = organisationType };
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>.Ignored))
                .Returns(Task.FromResult(transactionData));
            string returnUrl = null;

            // Act
            var result = await controller.RepresentingCompanyRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(expectedAction);
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task RepresentingCompanyRedirect_ShouldRedirectToType_WhenOrganisationTypeIsNull()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData { OrganisationType = null };
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>.Ignored))
                .Returns(Task.FromResult(transactionData));
            string returnUrl = null;

            // Act
            var result = await controller.RepresentingCompanyRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Type");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task RepresentingCompanyRedirect_ShouldRedirectToType_WhenTransactionDataIsNull()
        {
            // Arrange
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>.Ignored))
                .Returns(Task.FromResult<OrganisationTransactionData>(null));
            string returnUrl = null;

            // Act
            var result = await controller.RepresentingCompanyRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Type");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task RepresentingCompanyRedirect_ShouldRedirectToReturnUrl_WhenReturnUrlIsNotNull()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData { OrganisationType = ExternalOrganisationType.RegisteredCompany };
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>.Ignored))
                .Returns(Task.FromResult(transactionData));
            string returnUrl = "/organisation-found";

            // Act
            var result = await controller.RepresentingCompanyRedirect(returnUrl) as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            result.Url.Should().Be(returnUrl);
        }

        [Fact]
        public async Task ContactDetails_Get_WithExistingTransaction_ReturnsViewWithPopulatedViewModel()
        {
            // Arrange
            var countries = SetupCountries();

            var existingTransaction = new OrganisationTransactionData
            {
                ContactDetailsViewModel = TestFixture.Create<ContactDetailsViewModel>()
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.ContactDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as ContactDetailsViewModel;
            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(existingTransaction.ContactDetailsViewModel);
            model.HasAuthorisedRepresentitive.Should().Be(existingTransaction.ContactDetailsViewModel.HasAuthorisedRepresentitive);
            model.AddressData.Countries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public async Task ContactDetails_Get_WithoutExistingTransaction_ReturnsNewViewModel()
        {
            // Arrange
            var countries = SetupCountries();

            OrganisationTransactionData existingTransaction = null;

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.ContactDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as ContactDetailsViewModel;
            model.Should().NotBeNull();
            model.HasAuthorisedRepresentitive.Should().BeFalse();
            model.AddressData.Countries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public async Task ContactDetails_Post_ValidModel_RedirectsToType()
        {
            // Arrange
            var model = TestFixture.Create<ContactDetailsViewModel>();

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.ContactDetailsViewModel, model).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.ContactDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Type");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ContactDetails_Post_InValidModel_ReturnsView()
        {
            // Arrange
            var model = TestFixture.Create<ContactDetailsViewModel>();
            controller.ModelState.AddModelError("error", "error");

            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };

            A.CallTo(() =>
                    weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false)))
                .Returns(countries);

            // Act
            var result = await controller.ContactDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();

            var resultModel = result.Model as ContactDetailsViewModel;
            resultModel.Should().BeEquivalentTo(model);
            model.AddressData.Countries.Should().BeEquivalentTo(countries);
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustNotHaveHappened();
        }

        [Fact]
        public void RegistrationComplete_Get_ReturnsView()
        {
            var organisationId = TestFixture.Create<Guid>();

            // Act
            var result = controller.RegistrationComplete(organisationId) as ViewResult;

            // Assert
            Assert.True(string.IsNullOrEmpty(result.ViewName) || result.ViewName == "RegistrationComplete");
            ((Guid)result.Model).Should().Be(organisationId);
        }

        [Fact]
        public void RegistrationComplete_Post_RedirectsToHoldingController()
        {
            // Arrange
            var organisationId = TestFixture.Create<Guid>();

            // Act
            var result = controller.RegistrationCompleteSubmit(organisationId) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("ChooseActivity");
            result.RouteValues["controller"].Should().Be("Home");
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["pcsId"].Should().Be(organisationId);
        }

        [Fact]
        public async Task PartnerDetails_Get_ReturnsViewWithPopulatedViewModel()
        {
            // Act
            var result = (await controller.PartnerDetails()) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            var model = result.Model as PartnerViewModel;
            model.Should().NotBeNull();

            model.PartnerModels.Should().HaveCount(2);
            model.NotRequiredPartnerModels.Should().BeEmpty();
        }

        [Fact]
        public async Task PartnerDetails_Get_WithExistingTransaction_ReturnsViewWithSavedData()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "x", LastName = "y", Order = 1 },
                    new AdditionalContactModel { FirstName = "a", LastName = "b", Order = 1 },
                    new AdditionalContactModel { FirstName = "c", LastName = "d", Order = 2 },
                }
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = (await controller.PartnerDetails()) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            var model = result.Model as PartnerViewModel;
            model.Should().NotBeNull();

            model.PartnerModels.Should().HaveCount(2);
            model.PartnerModels.Should().BeEquivalentTo(organisationTransactionData.PartnerModels.Where(p => p.Order <= 1));
            model.NotRequiredPartnerModels.Should().HaveCount(1);
            model.NotRequiredPartnerModels.First().FirstName.Should().Be("c");
            model.NotRequiredPartnerModels.First().LastName.Should().Be("d");
        }

        [Theory]
        [InlineData(ExternalOrganisationType.RegisteredCompany, "Type")]
        [InlineData(ExternalOrganisationType.Partnership, "PartnerDetails")]
        [InlineData(ExternalOrganisationType.SoleTrader, "SoleTraderDetails")]
        public void PreviousPage_Get_RedirectsToPartnershipPageIfPartnerType(ExternalOrganisationType organisationType,
            string expectedAction)
        {
            // Act
            var result = controller.PreviousPage(organisationType) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be(expectedAction);
        }

        [Fact]
        public async Task GetAddresses_Get_GetsAddresses()
        {
            // Act
            var postcode = "ui7 5yhh";

            var result = await controller.GetAddresses(postcode) as JsonResult;

            result.Should().BeOfType<JsonResult>();
            result.JsonRequestBehavior.Should().Be(JsonRequestBehavior.AllowGet);

            A.CallTo(() =>
              addressLookupClient.GetAddressesAsync(
                  configurationService.CurrentConfiguration.AddressLookupReferencePath,
                  postcode))
                .MustHaveHappenedOnceExactly();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task FindCompany_Get_GetsCompany()
        {
            // Act
            var input = "456";

            var result = await controller.FindCompany(input) as JsonResult;

            result.Should().BeOfType<JsonResult>();
            result.JsonRequestBehavior.Should().Be(JsonRequestBehavior.AllowGet);

            A.CallTo(() =>
              companiesHouseClient.GetCompanyDetailsAsync(
                  configurationService.CurrentConfiguration.CompaniesHouseReferencePath,
                  input))
                .MustHaveHappenedOnceExactly();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task PartnerDetails_Get_ReturnsViewWithSavedData()
        {
            // Arrange
            var organisationTransactionData = new OrganisationTransactionData()
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "x", LastName = "y" },
                    new AdditionalContactModel { FirstName = "a", LastName = "b" },
                    new AdditionalContactModel { FirstName = "c", LastName = "d" },
                }
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = (await controller.PartnerDetails()) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            var model = result.Model as PartnerViewModel;
            model.Should().NotBeNull();

            model.PartnerModels.Should().BeEquivalentTo(organisationTransactionData.PartnerModels);
        }

        [Fact]
        public async Task PartnerDetails_Post_CallsCaptureDataAndRedirectsToOrganisationDetails()
        {
            // Arrange
            var vm = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "xx", LastName = "x" }
                }
            };

            // Act
            var result = await controller.PartnerDetails(vm, null, null) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();

            A.CallTo(() => transactionService.CaptureData(A<string>._, vm)).MustHaveHappenedOnceExactly();

            result.RouteValues["action"].Should().Be("OrganisationDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
        }

        [Fact]
        public async Task PartnerDetails_Post_AddsAnotherPartner()
        {
            // Arrange
            var vm = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "xx", LastName = "x" }
                },
                NotRequiredPartnerModels = new List<NotRequiredPartnerModel>()
            };

            // Act
            var result = await controller.PartnerDetails(vm, PostActionConstant.PartnerPostAdd, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            A.CallTo(() => transactionService.CaptureData(A<string>._, vm)).MustNotHaveHappened();

            var model = result.Model as PartnerViewModel;

            model.AllPartnerModels.Should().HaveCount(2);

            model.NotRequiredPartnerModels.Should().HaveCount(1);
            model.NotRequiredPartnerModels.First().FirstName.Should().BeNull();
            model.NotRequiredPartnerModels.First().LastName.Should().BeNull();
        }

        [Fact]
        public async Task PartnerDetails_Post_RemovesPartner()
        {
            // Arrange
            var vm = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "xx", LastName = "x" }
                },
                NotRequiredPartnerModels = new List<NotRequiredPartnerModel>
                {
                    new NotRequiredPartnerModel { FirstName = "yy", LastName = "y" },
                    new NotRequiredPartnerModel { FirstName = "zz", LastName = "z" }
                }
            };

            // Act
            var result = await controller.PartnerDetails(vm, PostActionConstant.PartnerPostRemove, 0) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            A.CallTo(() => transactionService.CaptureData(A<string>._, vm)).MustNotHaveHappened();

            var model = result.Model as PartnerViewModel;

            model.AllPartnerModels.Should().HaveCount(2);
            model.NotRequiredPartnerModels.Should().HaveCount(1);
            model.NotRequiredPartnerModels.First().FirstName.Should().Be("zz");
            model.NotRequiredPartnerModels.First().LastName.Should().Be("z");
        }

        [Fact]
        public async Task PartnerDetails_Post_AddErrorIfMoreThanTenPartners()
        {
            // Arrange
            var vm = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "xx", LastName = "x" },
                    new AdditionalContactModel { FirstName = "yy", LastName = "y" }
                },
                NotRequiredPartnerModels = Enumerable.Range(0, 8).Select(i => new NotRequiredPartnerModel { FirstName = $"First{i}", LastName = $"Last{i}" }).ToList()
            };

            // Act
            var result = await controller.PartnerDetails(vm, PostActionConstant.PartnerPostAdd, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();

            A.CallTo(() => transactionService.CaptureData(A<string>._, vm)).MustNotHaveHappened();

            controller.ModelState["PartnerModels"].Errors.Count.Should().Be(1);

            controller.ModelState["PartnerModels"].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("A maximum of 10 partners are allowed");

            var model = result.Model as PartnerViewModel;
            model.AllPartnerModels.Should().HaveCount(10);
        }

        [Fact]
        public async Task SoleTraderDetails_Get_ReturnsViewWithPopulatedViewModel()
        {
            // Act
            var result = await controller.SoleTraderDetails() as ViewResult;

            // Assert
            var model = result.Model as SoleTraderViewModel;

            result.Should().NotBeNull();
            model.Should().NotBeNull();
            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
        }

        [Fact]
        public async Task SoleTraderDetails_Get_WithExistingTransaction_ReturnsViewWithPopulatedViewModel()
        {
            // Arrange
            var existingTransaction = new OrganisationTransactionData
            {
                SoleTraderViewModel = TestFixture.Create<SoleTraderViewModel>()
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            // Act
            var result = await controller.SoleTraderDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as SoleTraderViewModel;

            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(existingTransaction.SoleTraderViewModel);
        }

        [Fact]
        public async Task SoleTraderDetails_Post_CallsCaptureDataAndRedirectsToOrganisationDetails()
        {
            // Arrange
            var model = TestFixture.Create<SoleTraderViewModel>();

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.SoleTraderViewModel, model).Create();

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.SoleTraderDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("OrganisationDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            A.CallTo(() => transactionService.CaptureData(A<string>._, model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationDetails_Post_WithEmptyRegistrationNumber_ShouldNotCallApiForRegistrationNumberSearch()
        {
            // Arrange
            var model = new OrganisationViewModel
                { CompaniesRegistrationNumber = string.Empty, CompanyName = "Test Company" };

            // Act
            await controller.OrganisationDetails(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task OrganisationDetails_Post_WithValidRegistrationNumber_ShouldCallApiForRegistrationNumberSearch()
        {
            // Arrange
            var model = new OrganisationViewModel
            { CompaniesRegistrationNumber = "12345678", CompanyName = "Test Company", Address = new ExternalAddressData { CountryId = Guid.NewGuid() } };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                .Returns(Task.FromResult(new List<Core.Organisations.OrganisationData>()));

            model.CompaniesRegistrationNumber = "4567894";
            model.EEEBrandNames = "s";

            // Act
            await controller.OrganisationDetails(model);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task OrganisationDetails_Post_WhenRegistrationNumberSearchReturnsEmptyList_ShouldFallbackToNameSearch()
        {
            // Arrange
            var model = new OrganisationViewModel
            { CompaniesRegistrationNumber = "12345678", CompanyName = "Test Company", Address = new ExternalAddressData { CountryId = Guid.NewGuid() } };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                .Returns(Task.FromResult(new List<Core.Organisations.OrganisationData>()));

            model.CompaniesRegistrationNumber = "4567894";
            model.EEEBrandNames = "s";

            // Act
            await controller.OrganisationDetails(model);

            // Assert
            A.CallTo(() => organisationSearcher.Search(model.CompanyName, A<int>._, A<bool>._))
                .MustHaveHappened();
        }

        [Fact]
        public void OrganisationFound_Get_ReturnsViewWithPopulatedModel()
        {
            // Arrange
            var organisationExistsSearchResult = new OrganisationExistsSearchResult
            {
                Organisations = new List<OrganisationFoundViewModel>
                {
                    new OrganisationFoundViewModel
                    {
                        OrganisationName = "Test Organisation",
                        OrganisationId = Guid.NewGuid(),
                        CompanyRegistrationNumber = "123456789"
                    }
                },
                FoundType = OrganisationFoundType.CompanyNumber
            };

            controller.TempData["FoundOrganisations"] = organisationExistsSearchResult;

            // Act
            var result = controller.OrganisationFound() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("OrganisationFound");

            var model = result.Model as OrganisationsFoundViewModel;
            model.Should().NotBeNull();
            model.OrganisationFoundViewModels.Should().BeEquivalentTo(organisationExistsSearchResult.Organisations);
            model.OrganisationFoundType.Should().Be(organisationExistsSearchResult.FoundType);
            controller.TempData["FoundOrganisations"].Should().Be(organisationExistsSearchResult);
        }

        [Fact]
        public void OrganisationFound_Post_ValidModelRedirectsToJoinOrganisation()
        {
            // Arrange
            var viewModel = new OrganisationsFoundViewModel
            {
                OrganisationFoundViewModels = new List<OrganisationFoundViewModel>
                {
                    new OrganisationFoundViewModel
                    {
                        OrganisationName = "Test Organisation",
                        OrganisationId = Guid.NewGuid(),
                        CompanyRegistrationNumber = "123456789"
                    }
                },
                SelectedOrganisationId = Guid.NewGuid(),
                OrganisationFoundType = OrganisationFoundType.CompanyNumber
            };

            // Act
            var result = controller.OrganisationFound(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("JoinOrganisation");
        }

        [Fact]
        public void OrganisationFound_Post_InvalidModelReturnsView()
        {
            // Arrange
            var viewModel = TestFixture.Create<OrganisationsFoundViewModel>();
            controller.ModelState.AddModelError("error", "error");

            // Act
            var result = controller.OrganisationFound(viewModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();

            var resultModel = result.Model as OrganisationsFoundViewModel;
            resultModel.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async Task OrganisationDetails_Post_PreviousSchemeMember_NoProducerRegistrationNumber_AddsError()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = string.Empty;
            model.IsPreviousSchemeMember = true;
            model.OrganisationType = ExternalOrganisationType.RegisteredCompany;

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.No)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProducerRegistrationNumberRequest>._))
                .Returns(Task.FromResult(false));

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState["ProducerRegistrationNumber"].Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Enter a producer registration number");
        }

        [Fact]
        public async Task OrganisationDetails_Post_PreviousSchemeMember_ValidProducerRegistrationNumber_DoesNotAddError()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = "ValidRegistrationNumber";
            model.IsPreviousSchemeMember = true;
            model.OrganisationType = ExternalOrganisationType.RegisteredCompany;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.No)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProducerRegistrationNumberRequest>._))
                .Returns(Task.FromResult(true));

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.OrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RegistrationComplete");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task OrganisationDetails_Post_PreviousSchemeMember_InvalidProducerRegistrationNumber_AddsError()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = "InvalidRegistrationNumber";
            model.IsPreviousSchemeMember = true;
            model.OrganisationType = ExternalOrganisationType.RegisteredCompany;

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.No)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProducerRegistrationNumberRequest>._))
                .Returns(Task.FromResult(false));

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState["ProducerRegistrationNumber"].Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("This producer registration number does not exist");
        }

        [Fact]
        public async Task OrganisationDetails_Post_NotPreviousSchemeMember_ProducerRegistrationNumber_NoError()
        {
            // Arrange
            var model = TestFixture.Build<OrganisationViewModel>().Create();
            model.ProducerRegistrationNumber = "ValidRegistrationNumber";
            model.IsPreviousSchemeMember = false;
            model.OrganisationType = ExternalOrganisationType.RegisteredCompany;
            model.CompaniesRegistrationNumber = "4567894";

            var organisationTransactionData = TestFixture.Build<OrganisationTransactionData>()
                .With(o => o.AuthorisedRepresentative, YesNoType.No)
                .Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProducerRegistrationNumberRequest>._))
                .Returns(Task.FromResult(true));

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.OrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RegistrationComplete");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task OrganisationDetails_Get_WithNoExistingTransaction_ReturnsDefaultViewModel()
        {
            // Arrange
            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns<OrganisationTransactionData>(null);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as RegisteredCompanyDetailsViewModel;
            model.Should().NotBeNull();
            model.OrganisationType.Should().Be(ExternalOrganisationType.RegisteredCompany);
            model.Address.Countries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public async Task OrganisationDetails_Get_WithExistingTransaction_PopulatesViewModelCorrectly()
        {
            // Arrange
            var existingTransaction = new OrganisationTransactionData
            {
                OrganisationType = ExternalOrganisationType.RegisteredCompany,
                PreviousRegistration = PreviouslyRegisteredProducerType.YesPreviousSchemeMember,
                OrganisationViewModel = new OrganisationViewModel()
                {
                    OrganisationType = ExternalOrganisationType.RegisteredCompany,
                    CompanyName = "Test Company",
                    CompaniesRegistrationNumber = "4567894"
                }
            };

            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(existingTransaction);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            // Act
            var result = await controller.OrganisationDetails() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as RegisteredCompanyDetailsViewModel;
            model.Should().NotBeNull();
            model.CompanyName.Should().Be("Test Company");
            model.OrganisationType.Should().Be(ExternalOrganisationType.RegisteredCompany);
            model.IsPreviousSchemeMember.Should().BeTrue();
            model.Address.Countries.Should().BeEquivalentTo(countries);
        }

        [Fact]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_NoTransactionData_RedirectsToRepresentingCompanyDetails()
        {
            // Arrange
            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(Task.FromResult<OrganisationTransactionData>(null));

            const string returnUrl = "/test-return-url";

            // Act
            var result = await controller.CheckAuthorisedRepresentitiveAndRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RepresentingCompanyDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["returnUrl"].Should().Be(returnUrl);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustNotHaveHappened();
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_WithNoAuthorisedRepresentative_CompletesRegistrationAndRedirects()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false))
                .Returns(organisationId);

            // Act
            var result = await controller.CheckAuthorisedRepresentitiveAndRedirect(null) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RegistrationComplete");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["organisationId"].Should().Be(organisationId);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_WithAuthorisedRepresentative_RedirectsToRepresentingCompanyDetails()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.Yes
            };

            const string returnUrl = "/test-return-url";

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);

            // Act
            var result = await controller.CheckAuthorisedRepresentitiveAndRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RepresentingCompanyDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["returnUrl"].Should().Be(returnUrl);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustNotHaveHappened();
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("/some-return-url")]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_ReturnUrlHandledCorrectly(string returnUrl)
        {
            // Arrange
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.Yes
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);

            // Act
            var result = await controller.CheckAuthorisedRepresentitiveAndRedirect(returnUrl) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("RepresentingCompanyDetails");
            result.RouteValues["controller"].Should().Be("OrganisationRegistration");
            result.RouteValues["returnUrl"].Should().Be(returnUrl);
        }

        [Fact]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_CompletesTransaction_SetsCorrectOrganisationId()
        {
            // Arrange
            var expectedOrganisationId = Guid.NewGuid();
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false))
                .Returns(expectedOrganisationId);

            // Act
            var result = await controller.CheckAuthorisedRepresentitiveAndRedirect(null) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["organisationId"].Should().Be(expectedOrganisationId);
        }

        [Fact]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_WithNoAuthRep_InvalidatesCacheAfterCompletingTransaction()
        {
            // Arrange
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);

            // Act
            await controller.CheckAuthorisedRepresentitiveAndRedirect(null);

            // Assert
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => weeeCache.InvalidateOrganisationSearch())
                    .MustHaveHappenedOnceExactly());
        }

        [Theory]
        [InlineData(YesNoType.Yes)]
        [InlineData(null)]
        public async Task CheckAuthorisedRepresentitiveAndRedirect_WhenNotNo_DoesNotCompleteTransaction(YesNoType? authRepType)
        {
            // Arrange
            var transactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = authRepType
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(transactionData);

            // Act
            await controller.CheckAuthorisedRepresentitiveAndRedirect(null);

            // Assert
            A.CallTo(() => transactionService.CompleteTransaction(A<string>._, false))
                .MustNotHaveHappened();
            A.CallTo(() => weeeCache.InvalidateOrganisationSearch())
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData("2024-11-06", true)]  // currentDate >= featureEnabledDate (Should show the message)
        [InlineData("2024-11-08", false)] // currentDate < featureEnabledDate (Should not show the message)
        public async Task SearchResults_SetsShowSmallProducerMessageCorrectly(DateTime featureEnabledDate, bool expectedMessageFlag)
        {
            // Arrange
            var fakeResults = new List<OrganisationSearchResult>
            {
                new OrganisationSearchResult
                {
                    OrganisationId = new Guid("05DF9AE8-DACE-4173-A227-16933EB5D5F8"),
                    Name = "Test Company"
                }
            };

            var organisationSearcher = A.Fake<ISearcher<OrganisationSearchResult>>();
            A.CallTo(() => organisationSearcher.Search("testSearchTerm", 5, false))
                .Returns(fakeResults);

            A.CallTo(() => configurationService.CurrentConfiguration.SmallProducerFeatureEnabledFrom)
                .Returns(featureEnabledDate);

            var apiDate = featureEnabledDate;
            if (!expectedMessageFlag)
            {
                apiDate = featureEnabledDate.AddDays(-1);
            }

            var weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._))
                .Returns(apiDate);

            var controller = new OrganisationRegistrationController(
                () => weeeClient,
                organisationSearcher,
                configurationService,
                transactionService,
                weeeCache,
                () => companiesHouseClient,
                () => addressLookupClient);

            // Act
            var result = await controller.SearchResults("testSearchTerm");

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            var viewModel = viewResult.Model as SearchResultsViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(expectedMessageFlag, viewModel.ShowSmallProducerMessage);
        }

        public static IEnumerable<object[]> NpwdMigrationTestData()
        {
            yield return new object[] { true, true, true };
            yield return new object[] { true, true, false };
            yield return new object[] { true, false, false };
            yield return new object[] { false, true, true };
            yield return new object[] { false, true, false };
            yield return new object[] { false, false, false };
        }

        [Theory]
        [MemberData(nameof(NpwdMigrationTestData))]
        public async Task GetExistingOrganisations_ShouldHandleNpwdMigrationStatus(
        bool isRegistrationNumberSearch,
        bool npwdMigrated,
        bool npwdMigratedComplete)
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var companyName = "Test Company";
            var registrationNumber = "12345678";

            var model = new OrganisationViewModel
            {
                CompaniesRegistrationNumber = isRegistrationNumberSearch ? registrationNumber : string.Empty,
                CompanyName = companyName,
                Address = new ExternalAddressData
                {
                    CountryId = Guid.NewGuid(),
                    Address1 = "Test Address",
                    TownOrCity = "Test Town",
                    Postcode = "TE1 1ST"
                },
                EEEBrandNames = "Brand Name",
                Action = string.Empty
            };

            if (isRegistrationNumberSearch)
            {
                var orgData = new OrganisationData
                {
                    Id = organisationId,
                    Name = companyName,
                    CompanyRegistrationNumber = registrationNumber,
                    NpwdMigrated = npwdMigrated,
                    NpwdMigratedComplete = npwdMigratedComplete
                };

                A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                    .Returns(new List<OrganisationData> { orgData });
            }
            else
            {
                var searchResult = new OrganisationSearchResult
                {
                    OrganisationId = organisationId,
                    Name = companyName,
                    CompanyRegistrationNumber = registrationNumber,
                    NpwdMigrated = npwdMigrated,
                    NpwdMigratedComplete = npwdMigratedComplete
                };

                A.CallTo(() => weeeClient.SendAsync(A<string>._, A<OrganisationByRegistrationNumberValue>._))
                    .Returns(new List<OrganisationData>());

                A.CallTo(() => organisationSearcher.Search(companyName, A<int>._, false))
                    .Returns(new List<OrganisationSearchResult> { searchResult });
            }

            var organisationTransactionData = new OrganisationTransactionData
            {
                AuthorisedRepresentative = YesNoType.No
            };

            A.CallTo(() => transactionService.GetOrganisationTransactionData(A<string>._))
                .Returns(organisationTransactionData);

            // Act
            var result = await controller.OrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("OrganisationFound");

            var foundOrgs = controller.TempData["FoundOrganisations"] as OrganisationExistsSearchResult;
            foundOrgs.Should().NotBeNull();
            foundOrgs.FoundType.Should().Be(isRegistrationNumberSearch
                ? OrganisationFoundType.CompanyNumber
                : OrganisationFoundType.CompanyName);
            foundOrgs.Organisations.Should().ContainSingle();

            var foundOrg = foundOrgs.Organisations.First();
            foundOrg.Should().BeEquivalentTo(new
            {
                OrganisationId = organisationId,
                OrganisationName = companyName,
                CompanyRegistrationNumber = registrationNumber,
                NpwdMigrated = npwdMigrated,
                NpwdMigratedComplete = npwdMigratedComplete
            });
        }

        [Theory]
        [InlineData("buildingName", null, "buildingName")]
        [InlineData(null, "123", "123")]
        [InlineData("Manor House", "45", "45")]
        public async Task FindCompany_HandlesAddressFieldsCorrectly(string buildingName, string buildingNumber, string expectedAddress1)
        {
            // Arrange
            var apiModel = new DefraCompaniesHouseApiModel
            {
                Organisation = new Organisation
                {
                    Name = "Test Company",
                    RegistrationNumber = "12345",
                    RegisteredOffice = new RegisteredOffice
                    {
                        BuildingName = buildingName,
                        BuildingNumber = buildingNumber,
                        Street = "Test Street",
                        Town = "Test Town",
                        Postcode = "TE1 1ST",
                        Country = new Country { Name = "United Kingdom" }
                    }
                }
            };

            A.CallTo(() => companiesHouseClient.GetCompanyDetailsAsync(A<string>._, A<string>._))
                .Returns(apiModel);

            // Act
            var result = await controller.FindCompany("12345") as JsonResult;
            var model = result.Data as OrganisationViewModel;

            // Assert
            model.Address.Address1.Should().Be(expectedAddress1);
        }

        [Theory]
        [InlineData("buildingName", null, "buildingName")]
        [InlineData(null, "123", "123")]
        [InlineData("Manor House", "45", "45")]
        public async Task OrganisationDetails_Post_HandlesAddressFieldsCorrectly(string buildingName, string buildingNumber, string expectedAddress1)
        {
            // Arrange
            var model = new OrganisationViewModel
            {
                Action = "Find Company",
                CompaniesRegistrationNumber = "12345"
            };

            var apiModel = new DefraCompaniesHouseApiModel
            {
                Organisation = new Organisation
                {
                    RegisteredOffice = new RegisteredOffice
                    {
                        BuildingName = buildingName,
                        BuildingNumber = buildingNumber,
                        Country = new Country { Name = "United Kingdom" }
                    }
                }
            };

            A.CallTo(() => companiesHouseClient.GetCompanyDetailsAsync(A<string>._, A<string>._))
                .Returns(apiModel);

            var countries = new List<CountryData> { new CountryData { Id = Guid.NewGuid(), Name = "United Kingdom" } };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            // Act
            var result = await controller.OrganisationDetails(model) as ViewResult;
            var resultModel = result.Model as OrganisationViewModel;

            // Assert
            resultModel.Address.Address1.Should().Be(expectedAddress1);
        }

        [Fact]
        public async Task ContinueSmallProducerRegistration_Should_Call_TransactionServiceMethods()
        {
            // Arrange
            var organisationId = Guid.NewGuid();

            A.CallTo(() => transactionService.DeleteOrganisationTransactionData(A<string>._)).Returns(Task.CompletedTask);

            A.CallTo(() => transactionService.ContinueMigratedProducerTransactionData(A<string>._, organisationId))
                .Returns(Task.FromResult(new OrganisationTransactionData()));

            // Act
            var result = await controller.ContinueSmallProducerRegistration(organisationId) as RedirectToRouteResult;

            // Assert
            A.CallTo(() => transactionService.DeleteOrganisationTransactionData(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => transactionService.ContinueMigratedProducerTransactionData(A<string>._, organisationId)).MustHaveHappenedOnceExactly();

            result.Should().NotBeNull();
            result.RouteValues["searchTerm"].Should().Be(string.Empty);
            result.RouteValues["action"].Should().Be("TonnageType");
        }

        [Fact]
        public async Task JoinOrganisation_Should_Call_ContinueSmallProducerRegistration_When_NpwdMigrated_Is_True_And_NpwdMigratedComplete_Is_False()
        {
            // Arrange
            var organisationId = Guid.NewGuid();

            var publicOrganisationData = new PublicOrganisationData
            {
                NpwdMigrated = true,
                NpwdMigratedComplete = false,
                DisplayName = "Test Organisation"
            };

            var getPublicOrganisationInfoRequest = new GetPublicOrganisationInfo(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetPublicOrganisationInfo>.That.Matches(r => r.Id == organisationId)))
                .Returns(Task.FromResult(publicOrganisationData));

            A.CallTo(() => transactionService.DeleteOrganisationTransactionData(A<string>.Ignored)).Returns(Task.CompletedTask);
            A.CallTo(() => transactionService.ContinueMigratedProducerTransactionData(A<string>.Ignored, A<Guid>.Ignored))
                .Returns(Task.FromResult(new OrganisationTransactionData()));

            // Act
            var result = await controller.JoinOrganisation(organisationId) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["searchTerm"].Should().Be(string.Empty);
            result.RouteValues["action"].Should().Be("TonnageType");
        }

        [Fact]
        public async Task JoinOrganisation_Should_Not_Call_ContinueSmallProducerRegistration_When_NpwdMigrated_Is_False_Or_NpwdMigratedComplete_Is_True()
        {
            // Arrange
            var organisationId = Guid.NewGuid();

            var publicOrganisationData = new PublicOrganisationData
            {
                NpwdMigrated = false,
                NpwdMigratedComplete = true,
                DisplayName = "Test Organisation"
            };

            // Arrange
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetPublicOrganisationInfo>._))
                .Returns(publicOrganisationData);

            var organisationSearcher = A.Dummy<ISearcher<OrganisationSearchResult>>();

            // Act
            ActionResult result = await controller.JoinOrganisation(organisationId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<JoinOrganisationViewModel>();
        }
    }
}