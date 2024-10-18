namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Controllers;
    using EA.Weee.Web.ViewModels.Organisation;
    using EA.Weee.Web.ViewModels.Organisation.Mapping.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class OrganisationControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly OrganisationController controller;

        public OrganisationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            controller = new OrganisationController(() => weeeClient, mapper);
        }

        [Fact]
        public async Task GetIndex_OneActiveOrg_RedirectsToSchemeChooseActivity()
        {
            // Arrange
            Guid organisationId = new Guid("433DB128-12A1-44FB-B26A-8128F8E36598");

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        OrganisationId = organisationId,
                        UserStatus = UserStatus.Active,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    }
                });

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);

            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;

            Assert.Equal("scheme", redirectResult.RouteValues["area"] as string, ignoreCase: true);
            Assert.Equal("home", redirectResult.RouteValues["controller"] as string, ignoreCase: true);
            Assert.Equal("chooseactivity", redirectResult.RouteValues["action"] as string, ignoreCase: true);
            Assert.Equal(organisationId, redirectResult.RouteValues["pcsId"]);
        }

        [Fact]
        public async Task GetIndex_OneActiveOrg_And_OneInactiveOrg_ShowsYourOrganisationsView()
        {
            // Arrange
            Guid organisationId1 = new Guid("433DB128-12A1-44FB-B26A-8128F8E36598");
            Guid organisationId2 = new Guid("90F0D10D-BE4E-462C-A214-30B94C461F8B");

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        OrganisationId = organisationId1,
                        UserStatus = UserStatus.Active,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    },
                    new OrganisationUserData
                    {
                        OrganisationId = organisationId2,
                        UserStatus = UserStatus.Pending,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 2",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    }
                });

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);

            ViewResult viewResult = result as ViewResult;

            Assert.Equal("YourOrganisations", viewResult.ViewName);

            Assert.IsAssignableFrom<YourOrganisationsViewModel>(viewResult.Model);

            YourOrganisationsViewModel model = viewResult.Model as YourOrganisationsViewModel;

            Assert.Single(model.Organisations);

            Assert.Equal(organisationId1, model.Organisations[0].OrganisationId);
            Assert.Equal("Organisation Name 1", model.Organisations[0].Organisation.OrganisationName);

            Assert.IsAssignableFrom<IEnumerable<OrganisationUserData>>(viewResult.ViewBag.InaccessibleOrganisations);

            IEnumerable<OrganisationUserData> inaccessibleOrganisations =
                viewResult.ViewBag.InaccessibleOrganisations as IEnumerable<OrganisationUserData>;

            Assert.Single(inaccessibleOrganisations);

            Assert.Equal(organisationId2, inaccessibleOrganisations.First().OrganisationId);
            Assert.Equal("Organisation Name 2", inaccessibleOrganisations.First().Organisation.OrganisationName);
        }

        [Fact]
        public async Task GetIndex_OneInactiveOrg_ShowsPendingView()
        {
            // Arrange
            Guid organisationId = new Guid("433DB128-12A1-44FB-B26A-8128F8E36598");

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        OrganisationId = organisationId,
                        UserStatus = UserStatus.Pending,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    }
                });

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);

            ViewResult viewResult = result as ViewResult;

            Assert.Equal("PendingOrganisations", viewResult.ViewName);

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);

            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Single(model.InaccessibleOrganisations);

            Assert.Equal(organisationId, model.InaccessibleOrganisations.First().OrganisationId);
            Assert.Equal("Organisation Name 1", model.InaccessibleOrganisations.First().Organisation.OrganisationName);
        }

        [Fact]
        public async Task GetIndex_NoOrganisations_RedirectsToCreateJoinOrganisation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>());

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);

            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;

            Assert.Null(redirectResult.RouteValues["area"] as string);
            Assert.Equal("organisationregistration", redirectResult.RouteValues["controller"] as string, ignoreCase: true);
            Assert.Equal("Search", redirectResult.RouteValues["action"] as string, ignoreCase: true);
        }

        /// <summary>
        /// This test ensures that the list of pending organisation associations will filter
        /// out duplicates. For a user who has two associations with the same organisation
        /// with statues of "Pending" and "Rejected", only the pending association should be shown.
        /// </summary>
        [Fact]
        public async Task GetIndex_WithPendingAndRejectedAssociation_ShowsOnlyPendingAssociation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        OrganisationId = new Guid("EF4B78BA-3D73-4B99-A996-714677590A78"),
                        UserStatus = UserStatus.Pending,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    },
                    new OrganisationUserData
                    {
                        OrganisationId = new Guid("EF4B78BA-3D73-4B99-A996-714677590A78"),
                        UserStatus = UserStatus.Rejected,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    }
                });

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);
            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Single(model.InaccessibleOrganisations);
            OrganisationUserData result1 = model.InaccessibleOrganisations.First();

            Assert.Equal(UserStatus.Pending, result1.UserStatus);
        }

        /// <summary>
        /// This test ensures that the list of pending organisation associations will filter
        /// out duplicates. For a user who has two associations with the same organisation
        /// with statues of "Inactive" and "Rejected", only the inactive association should be shown.
        /// </summary>
        [Fact]
        public async Task GetIndex_WithInactiveAndRejectedAssociation_ShowsOnlyInactiveAssociation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData
                    {
                        OrganisationId = new Guid("EF4B78BA-3D73-4B99-A996-714677590A78"),
                        UserStatus = UserStatus.Inactive,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    },
                    new OrganisationUserData
                    {
                        OrganisationId = new Guid("EF4B78BA-3D73-4B99-A996-714677590A78"),
                        UserStatus = UserStatus.Rejected,
                        Organisation = new OrganisationData
                        {
                            OrganisationName = "Organisation Name 1",
                            OrganisationStatus = OrganisationStatus.Complete,
                        }
                    }
                });

            OrganisationController controller = new OrganisationController(() => weeeClient, mapper);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);
            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Single(model.InaccessibleOrganisations);
            OrganisationUserData result1 = model.InaccessibleOrganisations.First();

            Assert.Equal(UserStatus.Inactive, result1.UserStatus);
        }

        [Fact]
        public async Task GetRepresentingCompanies_MapsCorrectly()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var organisationInfo = new OrganisationData { Id = organisationId };
            var organisations = new List<OrganisationUserData> { CreateOrganisationUserData(organisationId, UserStatus.Active) };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(organisationInfo);

            SetupWeeeClientWithOrganisations(organisations.ToArray());

            var expectedModel = new RepresentingCompaniesViewModel();
            A.CallTo(() => mapper.Map<RepresentingCompaniesViewModelMapSource, RepresentingCompaniesViewModel>(A<RepresentingCompaniesViewModelMapSource>._))
                .Returns(expectedModel);

            // Act
            var result = await controller.RepresentingCompanies(organisationId) as ViewResult;

            // Assert
            result.Model.Should().Be(expectedModel);

            A.CallTo(() => mapper.Map<RepresentingCompaniesViewModelMapSource, RepresentingCompaniesViewModel>(A<RepresentingCompaniesViewModelMapSource>.That.Matches(
                m => m.OrganisationData == organisationInfo && m.OrganisationsData.SequenceEqual(organisations)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetRepresentingOrganisation_ReturnsViewWithModel()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var countries = TestFixture.CreateMany<CountryData>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            // Act
            var result = await controller.RepresentingOrganisation(organisationId);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<RepresentingCompanyDetailsViewModel>()
                .Which.Should().Match<RepresentingCompanyDetailsViewModel>(m =>
                    m.OrganisationId == organisationId &&
                    m.Address.Countries == countries);
        }

        [Fact]
        public async Task PostRepresentingOrganisation_WithValidModel_RedirectsToChooseActivity()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var directRegistrantId = Guid.NewGuid();
            var model = new RepresentingCompanyDetailsViewModel { OrganisationId = organisationId };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddRepresentingCompany>._))
                .Returns(directRegistrantId);

            // Act
            var result = await controller.RepresentingOrganisation(model);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>()
                .Which.RouteValues.Should().ContainKeys("action", "controller", "area", "pcsId", "directRegistrantId")
                .And.ContainValues("ChooseActivity", "Home", "Scheme", organisationId, directRegistrantId);
        }

        [Fact]
        public async Task PostRepresentingOrganisation_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var model = new RepresentingCompanyDetailsViewModel { OrganisationId = organisationId };
            var countries = TestFixture.CreateMany<CountryData>().ToList();

            controller.ModelState.AddModelError("TestError", "This is a test error");

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            // Act
            var result = await controller.RepresentingOrganisation(model) as ViewResult;

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<RepresentingCompanyDetailsViewModel>()
                .Which.Should().Match<RepresentingCompanyDetailsViewModel>(m =>
                    m.OrganisationId == organisationId &&
                    m.Address.Countries == countries);
        }

        private void SetupWeeeClientWithOrganisations(params OrganisationUserData[] organisations)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(organisations.ToList());
        }

        private OrganisationUserData CreateOrganisationUserData(Guid organisationId, UserStatus status)
        {
            return new OrganisationUserData
            {
                OrganisationId = organisationId,
                UserStatus = status,
                Organisation = new OrganisationData
                {
                    OrganisationName = $"Organisation {organisationId}",
                    OrganisationStatus = OrganisationStatus.Complete,
                }
            };
        }
    }
}
