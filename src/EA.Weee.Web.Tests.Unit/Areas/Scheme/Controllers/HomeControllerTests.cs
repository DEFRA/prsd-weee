﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.Organisations;
    using Core.Users;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Users;
    using Weee.Requests.Users.GetManageableOrganisationUsers;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async void GetChooseActivity_ChecksForValidityOfOrganisation()
        {
            try
            {
                await HomeController().ChooseActivity(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetChooseActivity_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ChooseActivity(A<Guid>._));
        }

        [Fact]
        public async void GetChooseActivity_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseActivity(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetChooseActivity_DoNotHaveOrganisationUser_ReturnsViewWithOnlyThreeOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
               .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetManageableOrganisationUsers>._))
               .Returns(new List<OrganisationUserData>());

            var result = await HomeController().ChooseActivity(A<Guid>._);

            var model = (ChooseActivityViewModel)((ViewResult)result).Model;

            Assert.Equal(model.ActivityOptions.PossibleValues.Count, 3);

            Assert.False(model.ActivityOptions.PossibleValues.Contains(PcsAction.ManageOrganisationUsers));

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetChooseActivity_HaveOrganisationUser_ReturnsViewWithFourOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
               .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetManageableOrganisationUsers>._))
               .Returns(new List<OrganisationUserData>
               {
                   new OrganisationUserData
                   {
                       UserId = Guid.NewGuid().ToString()
                   },
                   new OrganisationUserData
                   {
                       UserId = Guid.NewGuid().ToString()
                   }
               });

            var result = await HomeController().ChooseActivity(A<Guid>._);

            var model = (ChooseActivityViewModel)((ViewResult)result).Model;

            Assert.Equal(model.ActivityOptions.PossibleValues.Count, 4);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersApprovedStatus_RedirectsToMemberRegistrationSummary()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Approved);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = PcsAction.ManagePcsMembers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Summary", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersPendingStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Pending);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = PcsAction.ManagePcsMembers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AuthorisationRequired", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersRejectedStatus_RedirectsToAuthorisationRequired()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Rejected);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = PcsAction.ManagePcsMembers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AuthorisationRequired", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ManageOrganisationUsers_RedirectsToOrganisationUsersManagement()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = PcsAction.ManageOrganisationUsers
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
        }

        [Fact]
        public async void GetManageOrganisationUsers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ManageOrganisationUsers(A<Guid>._));
        }

        [Fact]
        public async void GetManageOrganisationUsers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetManageableOrganisationUsers>._))
                .Returns(new List<OrganisationUserData>
                {
                    new OrganisationUserData(),
                });

            var result = await HomeController().ManageOrganisationUsers(A<Guid>._);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ManageOrganisationUsers");
        }

        [Fact]
        public async void PostManageOrganisationUsers_ModalStateValid_ReturnsView()
        {
            var model = new OrganisationUsersViewModel
            {
                OrganisationUsers =
                    new StringGuidRadioButtons
                    {
                        PossibleValues =
                            new[] { new RadioButtonPair<string, Guid>("User (UserStatus)", Guid.NewGuid()), },
                        Selected = new RadioButtonPair<string, Guid>("User (UserStatus)", Guid.NewGuid()),
                        SelectedValue = Guid.NewGuid()
                    }
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var result = await HomeController().ManageOrganisationUsers(A<Guid>._, model);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUser", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
            Assert.True(isModelStateValid);
        }

        [Fact]
        public async void GetManageOrganisationUser_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ManageOrganisationUser(A<Guid>._, A<Guid>._));
        }

        [Fact]
        public async void GetManageOrganisationUser_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationUser>._))
                .Returns(new OrganisationUserData
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserStatus = UserStatus.Active,
                    User = new UserData()
                });

            var result = await HomeController().ManageOrganisationUser(A<Guid>._, A<Guid>._);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ManageOrganisationUser");
        }

        [Fact]
        public async void GetManageOrganisationUser_NullOrganisationUserId_RedirectsToManageOrganisationUsers()
        {
            var result = await HomeController().ManageOrganisationUser(A.Dummy<Guid>(), (Guid?)null);

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostManageOrganisationUser_DoNotChangeSelected_MustNotHappendUpdateOrganisationUserStatus()
        {
            const string DoNotChange = "Do not change at this time";

            var model = new OrganisationUserViewModel
            {
                UserStatus = UserStatus.Active,
                UserId = Guid.NewGuid().ToString(),
                Firstname = "Test",
                Lastname = "Test",
                Username = "test@test.com",
                UserStatuses = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = DoNotChange,
                    PossibleValues = new[] { "Inactive", DoNotChange }
                }
            };

            await HomeController().ManageOrganisationUser(Guid.NewGuid(), model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationUserStatus>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void PostManageOrganisationUser_ModalStateValid_ReturnsView()
        {
            const string DoNotChange = "Do not change at this time";

            var model = new OrganisationUserViewModel
            {
                UserStatus = UserStatus.Active,
                UserId = Guid.NewGuid().ToString(),
                Firstname = "Test",
                Lastname = "Test",
                Username = "test@test.com",
                UserStatuses = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = "Inactive",
                    PossibleValues = new[] { "Inactive", DoNotChange }
                }
            };

            var result = await HomeController().ManageOrganisationUser(Guid.NewGuid(), model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationUserStatus>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
            Assert.Equal("Home", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_ViewOrganisationDetails_RedirectsToViewOrganisationDetails()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                ActivityOptions = new RadioButtonStringCollectionViewModel
                {
                    SelectedValue = PcsAction.ViewOrganisationDetails
                }
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewOrganisationDetails", routeValues["action"]);
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ManageOrganisationUsers(A<Guid>._));
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await HomeController().ViewOrganisationDetails(A<Guid>._);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ViewOrganisationDetails");
        }

        [Fact]
        public void PostViewOrganisationDetails_RedirectToChooseActivityView()
        {
            var result = HomeController().ViewOrganisationDetails(A<Guid>._, new ViewOrganisationDetailsViewModel());

            Assert.IsType<RedirectToRouteResult>(result);
            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", routeValues["action"]);
        }

        private HomeController HomeController()
        {
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        /// <summary>
        /// This test ensures that a GET request to the "ManageContactDetails" action of the Home controller will
        /// fetch the organisation data and the list of countries, set the countries on the organsiation's address
        /// and then return the default view with the organisation data as the model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetManageContactDetails_WithValidOrganisationId_GetsDataAndGetsCountriesAndReturnsDefaultView()
        {
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            OrganisationData organisationData = new OrganisationData();
            organisationData.Contact = new ContactData();
            organisationData.OrganisationAddress = new AddressData();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(organisationData);

            List<CountryData> countries = new List<CountryData>();
            
            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            Func<IWeeeClient> apiClient = () => client;

            IWeeeCache cache = A.Dummy<IWeeeCache>();

            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            HomeController controller = new HomeController(apiClient, cache, breadcrumb);
            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.ManageContactDetails(new Guid("A4B50C6B-64FE-4119-ACDF-82C502B59BC8"));

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(countries, organisationData.OrganisationAddress.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);
            
            ViewResult viewResult = (ViewResult)result;

            Assert.Equal(string.Empty, viewResult.ViewName);
            Assert.Equal(organisationData, viewResult.Model);
        }

        /// <summary>
        /// This test ensures that a POST request to the "ManageContactDetails" action of the Home controller where
        /// the post data results in a model error will fetch the list of countries, set the countries on the
        /// organsiation's address and then return the default view with the organisation data as the model.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostManageContactDetails_WithModelErrors_GetsCountriesAndReturnsDefaultView()
        {
            // Arrange
            OrganisationData organisationData = new OrganisationData();
            organisationData.Contact = new ContactData();
            organisationData.OrganisationAddress = new AddressData();

            IWeeeClient client = A.Fake<IWeeeClient>();

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            Func<IWeeeClient> apiClient = () => client;

            IWeeeCache cache = A.Dummy<IWeeeCache>();

            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            HomeController controller = new HomeController(apiClient, cache, breadcrumb);
            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("SomeProperty", "IsInvalid");

            // Act
            ActionResult result = await controller.ManageContactDetails(organisationData);

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(countries, organisationData.OrganisationAddress.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);

            ViewResult viewResult = (ViewResult)result;

            Assert.Equal(string.Empty, viewResult.ViewName);
            Assert.Equal(organisationData, viewResult.Model);
        }

        /// <summary>
        /// This test ensures that a POST request to the "ManageContactDetails" action of the Home controller where
        /// the post data results in no model errors will call the API to update the organisation details and
        /// then return a redirect result to the activity springboard page.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostManageContactDetails_WithNoModelErrors_UpdatesDetailsAndRedirectsToActivitySpringboard()
        {
            // Arrange
            OrganisationData organisationData = new OrganisationData();
            organisationData.Contact = new ContactData();
            organisationData.OrganisationAddress = new AddressData();

            IWeeeClient client = A.Fake<IWeeeClient>();

            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateOrganisationContactDetails>._))
                .Returns(true);

            Func<IWeeeClient> apiClient = () => client;

            IWeeeCache cache = A.Dummy<IWeeeCache>();

            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            HomeController controller = new HomeController(apiClient, cache, breadcrumb);
            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.ManageContactDetails(organisationData);

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateOrganisationContactDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType(typeof(RedirectToRouteResult), result);

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.Equal("ChooseActivity", redirectResult.RouteValues["Action"]);
        }
    }
}
