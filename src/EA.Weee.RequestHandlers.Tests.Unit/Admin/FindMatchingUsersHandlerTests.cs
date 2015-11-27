namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class FindMatchingUsersHandlerTests
    {
        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to find matching users.
        /// </summary>
        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void FindMatchingUsersHandler_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrage
            IFindMatchingUsersDataAccess dataAccess = A.Fake<IFindMatchingUsersDataAccess>();
            A.CallTo(() => dataAccess.GetOrganisationUsers()).Returns(new UserSearchData[5]);
            A.CallTo(() => dataAccess.GetCompetentAuthorityUsers()).Returns(new UserSearchData[5]);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            FindMatchingUsersHandler handler = new FindMatchingUsersHandler(authorization, dataAccess);
            
            FindMatchingUsers request = new FindMatchingUsers(1, 1, FindMatchingUsers.OrderBy.FullNameAscending);

            // Act
            Func<Task<UserSearchDataResult>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        /// <summary>
        /// This test ensures that an internal user can execute requests to find maatching users.
        /// </summary>
        [Fact]
        [Trait("Authorization", "Internal")]
        public async void FindMatchingUsersHandler_WithInternalUser_DoesntThrowException()
        {
            // Arrage
            IFindMatchingUsersDataAccess dataAccess = CreateFakeDataAccess();

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            FindMatchingUsersHandler handler = new FindMatchingUsersHandler(authorization, dataAccess);

            FindMatchingUsers request = new FindMatchingUsers(1, 1, FindMatchingUsers.OrderBy.FullNameAscending);

            // Act
            await handler.HandleAsync(request);

            // Assert
            // no exception
        }

        /// <summary>
        /// This test ensures that the results are correctly paged.
        /// </summary>
        [Fact]
        public async void FindMatchingUsersHandler_RequestingSpecificPage_ReturnsCorrectNumberOfResults()
        {
            // Arrage

            IFindMatchingUsersDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            FindMatchingUsersHandler handler = new FindMatchingUsersHandler(authorization, dataAccess);

            // Page 2, where each page has 3 results.
            FindMatchingUsers request = new FindMatchingUsers(2, 3, FindMatchingUsers.OrderBy.FullNameAscending); 

            // Act
            var response = await handler.HandleAsync(request);

            // Assert - Check that there are 10 users in total.
            Assert.NotNull(response);
            Assert.Equal(10, response.UsersCount);

            // Assert - We asked for page 2 with 3 results, so check that only 3 results are returned.
            Assert.NotNull(response.Results);
            Assert.Equal(3, response.Results.Count);
        }

        /// <summary>
        /// This test ensures that the results are correctly sorted before being returned.
        /// </summary>
        [Fact]
        public async void FindMatchingUsersHandler_WithFullNameAscendingOrdering_ReturnsResultsSortedByFullName()
        {
            // Arrage
            IFindMatchingUsersDataAccess dataAccess = CreateFakeDataAccess();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            FindMatchingUsersHandler handler = new FindMatchingUsersHandler(authorization, dataAccess);

            FindMatchingUsers request = new FindMatchingUsers(1, 1000, FindMatchingUsers.OrderBy.FullNameAscending);

            // Act
            var response = await handler.HandleAsync(request);

            // Check the first and last results have the correct IDs.
            Assert.NotNull(response);
            Assert.Equal("AGF GUI", response.Results.First().FullName);
            Assert.Equal("YGR FTW", response.Results.Last().FullName);
        }

        /// <summary>
        /// Creates a fake data access that returns 5 organisation users and 5 competent authority users.
        /// </summary>
        /// <returns></returns>
        private IFindMatchingUsersDataAccess CreateFakeDataAccess()
        {
            IFindMatchingUsersDataAccess dataAccess = A.Fake<IFindMatchingUsersDataAccess>();

            List<UserSearchData> organisationUsers = new List<UserSearchData>()
                {
                    new UserSearchData() { Id = "User 1", FirstName = "XGF", LastName = "RYH" },
                    new UserSearchData() { Id = "User 2", FirstName = "RHY", LastName = "EGJ" },
                    new UserSearchData() { Id = "User 3", FirstName = "GDR", LastName = "FDV" },
                    new UserSearchData() { Id = "User 4", FirstName = "JUK", LastName = "EEE" },
                    new UserSearchData() { Id = "User 5", FirstName = "HBN", LastName = "UTL" },
                };

            A.CallTo(() => dataAccess.GetOrganisationUsers()).Returns(organisationUsers.ToArray());

            List<UserSearchData> competentAuthorityUsers = new List<UserSearchData>()
                {
                    new UserSearchData() { Id = "User 6", FirstName = "AGF", LastName = "GUI" },
                    new UserSearchData() { Id = "User 7", FirstName = "HTF", LastName = "HBG" },
                    new UserSearchData() { Id = "User 8", FirstName = "VFE", LastName = "RDE" },
                    new UserSearchData() { Id = "User 9", FirstName = "TED", LastName = "SWR" },
                    new UserSearchData() { Id = "User 10", FirstName = "YGR", LastName = "FTW" },
                };

            A.CallTo(() => dataAccess.GetCompetentAuthorityUsers()).Returns(competentAuthorityUsers.ToArray());

            return dataAccess;
        }
    }
}
