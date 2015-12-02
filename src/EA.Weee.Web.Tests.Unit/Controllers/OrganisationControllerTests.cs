namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Web.Controllers;
    using EA.Weee.Web.ViewModels.Organisation;
    using FakeItEasy;
    using Xunit;

    public class OrganisationControllerTests
    {
        [Fact]
        public async void GetIndex_OneActiveOrg_RedirectsToSchemeChooseActivity()
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

            OrganisationController controller = new OrganisationController(() => weeeClient);

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
        public async void GetIndex_OneActiveOrg_And_OneInactiveOrg_ShowsYourOrganisationsView()
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

            OrganisationController controller = new OrganisationController(() => weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);

            ViewResult viewResult = result as ViewResult;

            Assert.Equal("YourOrganisations", viewResult.ViewName);

            Assert.IsAssignableFrom<YourOrganisationsViewModel>(viewResult.Model);

            YourOrganisationsViewModel model = viewResult.Model as YourOrganisationsViewModel;

            Assert.Equal(1, model.AccessibleOrganisations.PossibleValues.Count);

            Assert.Equal(organisationId1, model.AccessibleOrganisations.PossibleValues[0].Value);
            Assert.Equal("Organisation Name 1", model.AccessibleOrganisations.PossibleValues[0].Key);

            Assert.IsAssignableFrom<IEnumerable<OrganisationUserData>>(viewResult.ViewBag.InaccessibleOrganisations);

            IEnumerable<OrganisationUserData> inaccessibleOrganisations =
                viewResult.ViewBag.InaccessibleOrganisations as IEnumerable<OrganisationUserData>;

            Assert.Equal(1, inaccessibleOrganisations.Count());

            Assert.Equal(organisationId2, inaccessibleOrganisations.First().OrganisationId);
            Assert.Equal("Organisation Name 2", inaccessibleOrganisations.First().Organisation.OrganisationName);
        }

        [Fact]
        public async void GetIndex_OneInactiveOrg_ShowsPendingView()
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

            OrganisationController controller = new OrganisationController(() => weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResult>(result);

            ViewResult viewResult = result as ViewResult;

            Assert.Equal("PendingOrganisations", viewResult.ViewName);

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);

            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Equal(1, model.InaccessibleOrganisations.Count());

            Assert.Equal(organisationId, model.InaccessibleOrganisations.First().OrganisationId);
            Assert.Equal("Organisation Name 1", model.InaccessibleOrganisations.First().Organisation.OrganisationName);
        }

        [Fact]
        public async void GetIndex_NoOrganisations_RedirectsToCreateJoinOrganisation()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserOrganisationsByStatus>._))
                .Returns(new List<OrganisationUserData>());

            OrganisationController controller = new OrganisationController(() => weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);

            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;

            Assert.Equal(null, redirectResult.RouteValues["area"] as string, ignoreCase: true);
            Assert.Equal("organisationregistration", redirectResult.RouteValues["controller"] as string, ignoreCase: true);
            Assert.Equal("Search", redirectResult.RouteValues["action"] as string, ignoreCase: true);
        }

        /// <summary>
        /// This test ensures that the list of pending organisation associations will filter
        /// out duplicates. For a user who has two associations with the same organisation
        /// with statues of "Pending" and "Rejected", only the pending association should be shown.
        /// </summary>
        [Fact]
        public async void GetIndex_WithPendingAndRejectedAssociation_ShowsOnlyPendingAssociation()
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

            OrganisationController controller = new OrganisationController(() => weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);
            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Equal(1, model.InaccessibleOrganisations.Count());
            OrganisationUserData result1 = model.InaccessibleOrganisations.First();

            Assert.Equal(UserStatus.Pending, result1.UserStatus);
        }

        /// <summary>
        /// This test ensures that the list of pending organisation associations will filter
        /// out duplicates. For a user who has two associations with the same organisation
        /// with statues of "Inactive" and "Rejected", only the inactive association should be shown.
        /// </summary>
        [Fact]
        public async void GetIndex_WithInactiveAndRejectedAssociation_ShowsOnlyInactiveAssociation()
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

            OrganisationController controller = new OrganisationController(() => weeeClient);

            // Act
            ActionResult result = await controller.Index();

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.IsAssignableFrom<PendingOrganisationsViewModel>(viewResult.Model);
            PendingOrganisationsViewModel model = viewResult.Model as PendingOrganisationsViewModel;

            Assert.Equal(1, model.InaccessibleOrganisations.Count());
            OrganisationUserData result1 = model.InaccessibleOrganisations.First();

            Assert.Equal(UserStatus.Inactive, result1.UserStatus);
        }
    }
}
