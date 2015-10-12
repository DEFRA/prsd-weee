﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using FakeItEasy;
    using RequestHandlers.Admin.Submissions;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSubmissionsHistoryResultsHandlerTests
        {
            private readonly DbContextHelper dbContextHelper = new DbContextHelper();

            [Theory]
            [InlineData(AuthorizationBuilder.UserType.External)]
            [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
            public async Task GetAllComplianceYearsHandler_NotAdminUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
            {
                // Arrange
                Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");
                IGetSubmissionsHistoryResultsDataAccess dataAccess = A.Dummy<IGetSubmissionsHistoryResultsDataAccess>();
                IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

                GetSubmissionsHistoryResultsHandler handler = new GetSubmissionsHistoryResultsHandler(authorization, dataAccess);

                GetSubmissionsHistoryResults request = new GetSubmissionsHistoryResults(2016, pcsId);

                // Act
                Func<Task> action = async () => await handler.HandleAsync(request);

                // Asert
                await Assert.ThrowsAsync<SecurityException>(action);
            }
        }
}