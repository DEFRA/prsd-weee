﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Admin;
    using Core.Shared;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Weee.Requests.Admin;
    using Weee.Requests.Users;
    using Xunit;
    using GetUserData = Weee.Requests.Admin.GetUserData;

    public class UserControllerTests
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeClient weeeClient;
        public UserControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            apiClient = () => weeeClient;
        }

        [Fact]
        public async Task ManageUsersPost_ReturnsEditUserRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = new UserController(apiClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());

            var result = await controller.ManageUsers(new ManageUsersViewModel { SelectedUserId = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("EditUser", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["orgUserId"]);
        }

        [Fact]
        public async Task EditUsersGet_ReturnsView()
        {   
            var controller = new UserController(apiClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserData>._))
                .Returns(new ManageUserData
                {
                    UserStatus = UserStatus.Active,
                    OrganisationId = Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid().ToString(),
                    FirstName = "Firstname",
                    LastName = "Lastname",
                    Email = "test@ea.com",
                    IsCompetentAuthorityUser = true,
                    OrganisationName = "test ltd." 
                });

            var result = await controller.EditUser(Guid.NewGuid());

            var model = ((ViewResult)result).Model;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUserData>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsType<EditUserViewModel>(model);
        }

        [Fact]
        public async Task EditUsersPost_CompetentAuthorityUser_UpdateUserAndCompetentAuthorityUserStatusAndRedirectToManageUser()
        {
            var controller = new UserController(apiClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());

            var model = new EditUserViewModel
            {
                UserStatus = UserStatus.Active,
                OrganisationId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Firstname",
                LastName = "Lastname",
                Email = "test@ea.com",
                IsCompetentAuthorityUser = true,
                OrganisationName = "test ltd."
            };

            var result = await controller.EditUser(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateUser>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateCompetentAuthorityUserStatus>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("ManageUsers", redirectValues["action"]);
        }

        [Fact]
        public async Task EditUsersPost_OrganisationUser_UpdateUserAndOrganisationUserStatusAndRedirectToManageUser()
        {
            var controller = new UserController(apiClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());

            var model = new EditUserViewModel
            {
                UserStatus = UserStatus.Active,
                OrganisationId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Firstname",
                LastName = "Lastname",
                Email = "test@ea.com",
                IsCompetentAuthorityUser = false,
                OrganisationName = "test ltd."
            };

            var result = await controller.EditUser(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateUser>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationUserStatus>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("ManageUsers", redirectValues["action"]);
        }
    }
}
