namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.Submissions;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAllApprovedSchemesHandlerTests
        {
            private readonly DbContextHelper dbContextHelper = new DbContextHelper();

            [Theory]
            [InlineData(AuthorizationBuilder.UserType.External)]
            [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
            public async Task GetAllComplianceYearsHandler_NotAdminUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
            {
                // Arrange
                Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");
                IGetAllApprovedSchemesDataAccess dataAccess = A.Dummy<IGetAllApprovedSchemesDataAccess>();
                IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
                var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
                GetAllApprovedSchemesHandler handler = new GetAllApprovedSchemesHandler(authorization, schemeMap, dataAccess);

                GetAllApprovedSchemes request = new GetAllApprovedSchemes();

                // Act
                Func<Task> action = async () => await handler.HandleAsync(request);

                // Asert
                await Assert.ThrowsAsync<SecurityException>(action);
            }
        }
}
