namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Users;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Web.ViewModels.Shared.Scheme;
    using Web.ViewModels.Shared.Submission;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme.MemberRegistration;
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
                await HomeController().ChooseActivity(A.Dummy<Guid>());
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

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ChooseActivity(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetChooseActivity_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseActivity(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetActivities_DoesNotHaveMultipleOrganisationUsers_DoesNotReturnManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async Task GetActivities_HasMultipleOrganisationUsers_ReturnsManageOrganisationUsersOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMultipleOrganisationUsers = true
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageOrganisationUsers, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToFalse_DoesNotReturnManageEeeWeeeDataOption()
        {
            var result = await HomeController(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableDataReturnsConfigurationSetToTrue_ReturnsManageEeeWeeeDataOption()
        {
            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ManageEeeWeeeData, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToTrue_ReturnsAATFReturnOption()
        {
            var result = await HomeControllerSetupForAATFReturns(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.MakeAATFReturn, result);
        }

        [Fact]
        public async Task GetActivities_WithEnableAATFReturnsConfigurationSetToFalse_DoesNotReturnsAATFReturnOption()
        {
            var result = await HomeControllerSetupForAATFReturns(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.MakeAATFReturn, result);
        }

        [Fact]
        public async Task GetActivities_DoesNotHaveMemberSubmissions_AndDoesNotHaveDataReturnSubmissions_DoesNotReturnViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasMemberSubmissions_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = true,
                   HasDataReturnSubmissions = false
               });

            var result = await HomeController().GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasDataReturnSubmissions_AndEnableDataReturnsConfigurationSetToTrue_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = true
               });

            var result = await HomeController(true).GetActivities(A.Dummy<Guid>());

            Assert.Contains(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async Task GetActivities_HasDataReturnSubmissions_AndEnableDataReturnsConfigurationSetToFalse_ReturnsViewSubmissionHistoryOption()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
               .Returns(new OrganisationOverview
               {
                   HasMemberSubmissions = false,
                   HasDataReturnSubmissions = true
               });

            var result = await HomeController(false).GetActivities(A.Dummy<Guid>());

            Assert.DoesNotContain(PcsAction.ViewSubmissionHistory, result);
        }

        [Fact]
        public async void PostChooseActivity_ManagePcsMembersApprovedStatus_RedirectsToMemberRegistrationSummary()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Approved);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePcsMembers
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
                SelectedValue = PcsAction.ManagePcsMembers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("AuthorisationRequired", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Theory]
        [InlineData(SchemeStatus.Rejected)]
        [InlineData(SchemeStatus.Withdrawn)]
        public async void PostChooseActivity_ManagePcsMembersRejectedOrWithdrawnStatus_RedirectsToAuthorisationRequired(SchemeStatus status)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(status);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManagePcsMembers
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
                SelectedValue = PcsAction.ManageOrganisationUsers
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageOrganisationUsers", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseActivity_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageOrganisationUsers
            };
            var result = await controller.ChooseActivity(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void GetChooseSubmissionType_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ChooseSubmissionType(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetChooseSubmissionType_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseSubmissionType(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostChooseSubmissionType_ModelIsInvalid_ShouldRedirectViewWithModel()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("Key", "Any error");

            var model = new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            };
            var result = await controller.ChooseSubmissionType(model);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(model, ((ViewResult)(result)).Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void PostChooseSubmissionType_MemberRegistrationsSelected_RedirectsToMemberRegistrationSubmissionHistory()
        {
            var result = await HomeController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.MemberRegistrations
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async void PostChooseSubmissionType_DataReturnsSelected_RedirectsToDataReturnsSubmissionHistory()
        {
            var result = await HomeController().ChooseSubmissionType(new ChooseSubmissionTypeViewModel
            {
                SelectedValue = SubmissionType.EeeOrWeeeDataReturns
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewDataReturnSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async void GetManageOrganisationUsers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUsers(A.Dummy<Guid>()));
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

            var result = await HomeController().ManageOrganisationUsers(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ManageOrganisationUsers");
        }

        [Fact]
        public async void PostManageOrganisationUsers_ModalStateValid_ReturnsView()
        {
            var model = new OrganisationUsersViewModel
            {
                OrganisationUsers = new List<KeyValuePair<string, Guid>>
                {
                    new KeyValuePair<string, Guid>("User (UserStatus)", Guid.NewGuid()),
                    new KeyValuePair<string, Guid>("User (UserStatus)", Guid.NewGuid())
                },
                SelectedOrganisationUser = Guid.NewGuid()
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var result = await HomeController().ManageOrganisationUsers(A.Dummy<Guid>(), model);

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

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUser(A.Dummy<Guid>(), A.Dummy<Guid>()));
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

            var result = await HomeController().ManageOrganisationUser(A.Dummy<Guid>(), A.Dummy<Guid>());

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
                SelectedValue = DoNotChange,
                PossibleValues = new[] { "Inactive", DoNotChange }
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
                SelectedValue = "Inactive",
                PossibleValues = new[] { "Inactive", DoNotChange }
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
                SelectedValue = PcsAction.ViewRegisteredOfficeDetails
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewOrganisationDetails", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithEnableDataReturnsConfigurationSetToFalse_RedirectsToViewSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(false).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithNoSubmittedDataReturn_RedirectsToViewSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = false
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithNoSubmittedMemberUpload_RedirectsToViewDataReturnSubmissionHistory()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = false,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ViewDataReturnSubmissionHistory", routeValues["action"]);
        }

        [Fact]
        public async Task PostChooseActivity_SelectViewSubmissionHistory_WithSubmittedMemberUpload_AndSubmittedDataReturn_AndEnableDataReturnsConfigurationSetToTrue_RedirectsToChooseSubmissionType()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationOverview>._))
                .Returns(new OrganisationOverview
                {
                    HasMemberSubmissions = true,
                    HasDataReturnSubmissions = true
                });

            var result = await HomeController(true).ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ViewSubmissionHistory
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseSubmissionType", routeValues["action"]);
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAsync<ArgumentException>(() => HomeController().ManageOrganisationUsers(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetViewOrganisationDetails_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await HomeController().ViewOrganisationDetails(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ViewOrganisationDetails");
        }

        [Fact]
        public void PostViewOrganisationDetails_RedirectToChooseActivityView()
        {
            var result = HomeController().ViewOrganisationDetails(A.Dummy<Guid>(), new ViewOrganisationDetailsViewModel());

            Assert.IsType<RedirectToRouteResult>(result);
            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ChooseActivity", routeValues["action"]);
        }

        private HomeController HomeController(bool enableDataReturns = false)
        {
            ConfigurationService configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableDataReturns = enableDataReturns;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private HomeController HomeControllerSetupForAATFReturns(bool enableAATFReturns = false)
        {
            ConfigurationService configService = A.Fake<ConfigurationService>();
            configService.CurrentConfiguration.EnableAATFReturns = enableAATFReturns;
            var controller = new HomeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>(), A.Fake<CsvWriterFactory>(), configService);
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

            CsvWriterFactory csvWriterFactory = A.Dummy<CsvWriterFactory>();

            ConfigurationService configService = A.Dummy<ConfigurationService>();
            HomeController controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
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

            CsvWriterFactory csvWriterFactory = A.Dummy<CsvWriterFactory>();

            ConfigurationService configService = A.Dummy<ConfigurationService>();

            HomeController controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
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

            CsvWriterFactory csvWriterFactory = A.Dummy<CsvWriterFactory>();

            ConfigurationService configService = A.Dummy<ConfigurationService>();
            HomeController controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);
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

        [Fact]
        public async Task PostManageContactDetails_UpdatesDetailsAndSendNotificationOnChange()
        {
            // Arrange
            IWeeeClient client = A.Fake<IWeeeClient>();

            Func<IWeeeClient> apiClient = () => client;

            IWeeeCache cache = A.Dummy<IWeeeCache>();

            BreadcrumbService breadcrumb = A.Dummy<BreadcrumbService>();

            CsvWriterFactory csvWriterFactory = A.Dummy<CsvWriterFactory>();

            ConfigurationService configService = A.Dummy<ConfigurationService>();
            HomeController controller = new HomeController(apiClient, cache, breadcrumb, csvWriterFactory, configService);

            // Act
            ActionResult result = await controller.ManageContactDetails(A.Dummy<OrganisationData>());

            // Assert
            A.CallTo(() => client.SendAsync(A<string>._, A<UpdateOrganisationContactDetails>._))
                .WhenArgumentsMatch(a => ((UpdateOrganisationContactDetails)a[1]).SendNotificationOnChange == true)
                .MustHaveHappened();
        }

        [Fact]
        public async void GetViewSubmissionHistory_ShouldExecuteGetSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewSubmissionHistory_SchemeInfoIsNull_ShouldNotExecuteGetSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._)).Returns((SchemePublicInfo)null);

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .MustNotHaveHappened();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewSubmissionHistory_SortsByComplianceYearDescendingAsDefault()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).Ordering == SubmissionsHistoryOrderBy.ComplianceYearDescending)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as SubmissionHistoryViewModel;

            Assert.Equal(SubmissionsHistoryOrderBy.ComplianceYearDescending, model.OrderBy);
        }

        [Fact]
        public async void GetViewSubmissionHistory_DoesNotRequestForSummaryData()
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).IncludeSummaryData == false)
                .MustHaveHappened();
        }

        [Theory]
        [InlineData(SubmissionsHistoryOrderBy.ComplianceYearAscending)]
        [InlineData(SubmissionsHistoryOrderBy.ComplianceYearDescending)]
        [InlineData(SubmissionsHistoryOrderBy.SubmissionDateAscending)]
        [InlineData(SubmissionsHistoryOrderBy.SubmissionDateDescending)]
        public async void GetViewSubmissionHistory_SortsBySpecifiedValue(SubmissionsHistoryOrderBy orderBy)
        {
            var controller = HomeController();

            var result = await controller.ViewSubmissionHistory(A.Dummy<Guid>(), orderBy);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetSubmissionsHistoryResults>(1).Ordering == orderBy)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as SubmissionHistoryViewModel;

            Assert.Equal(orderBy, model.OrderBy);
        }

        [Fact]
        public async void GetDownloadCsv_ShouldExecuteGetMemberUploadDataAndReturnsCsvFile()
        {
            var controller = HomeController();

            var result = await controller.DownloadCsv(A.Dummy<Guid>(), A.Dummy<int>(), A.Dummy<Guid>(), A.Dummy<DateTime>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_ShouldExecuteGetDataReturnSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_SchemeInfoIsNull_ShouldNotExecuteGetDataReturnSubmissionsHistoryResultsAndReturnsView()
        {
            var controller = HomeController();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemePublicInfo>._)).Returns((SchemePublicInfo)null);

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .MustNotHaveHappened();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetViewDataReturnSubmissionHistory_SortsByComplianceYearDescendingAsDefault()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetDataReturnSubmissionsHistoryResults>(1).Ordering == DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as DataReturnSubmissionHistoryViewModel;

            Assert.Equal(DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending, model.OrderBy);
        }

        [Theory]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.QuarterAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.QuarterDescending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending)]
        [InlineData(DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending)]
        public async void GetViewDataReturnSubmissionHistory_SortsBySpecifiedValue(DataReturnSubmissionsHistoryOrderBy orderBy)
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>(), orderBy);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => a.Get<GetDataReturnSubmissionsHistoryResults>(1).Ordering == orderBy)
                .MustHaveHappened();

            var viewResult = result as ViewResult;
            var model = viewResult.Model as DataReturnSubmissionHistoryViewModel;

            Assert.Equal(orderBy, model.OrderBy);
        }

        [Fact]
        public async Task GetViewDataReturnSubmissionHistory_DoesNotRequestForSummaryData()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).IncludeSummaryData == false)
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetViewDataReturnSubmissionHistory_DoesNotRequestForEeeOutputDataComparison()
        {
            var controller = HomeController();

            var result = await controller.ViewDataReturnSubmissionHistory(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetDataReturnSubmissionsHistoryResults>._))
                .WhenArgumentsMatch(a => ((GetDataReturnSubmissionsHistoryResults)a[1]).CompareEeeOutputData == false)
                .MustHaveHappened();
        }

        [Fact]
        public async void PostChooseActivity_ManageEeeWeeeData_RedirectsToDataReturnsIndex()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._)).Returns(SchemeStatus.Approved);

            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.ManageEeeWeeeData
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("DataReturns", routeValues["controller"]);
        }

        [Fact]
        public async void PostChooseActivity_MakeAATFReturn_RedirectsToAatfTaskList()
        {
            var result = await HomeController().ChooseActivity(new ChooseActivityViewModel
            {
                SelectedValue = PcsAction.MakeAATFReturn
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
            Assert.Equal("AatfTaskList", routeValues["controller"]);
        }
    }
}
