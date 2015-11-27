namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Core.Scheme;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemeByIdHandlerTests
    {
        /// <summary>
        /// This test ensures that an authorized user can execute requests to get scheme data
        /// if they provide a valid scheme ID.
        /// </summary>
        [Fact]
        public async void GetSchemeByIdHandler_HappyPath_ReturnsSchemeData()
        {
            // Arrage
            Guid schemeId = new Guid("AC9116BC-5732-4F80-9AED-A6E2A0C4C1F1");

            IGetSchemeByIdDataAccess dataAccess = A.Fake<IGetSchemeByIdDataAccess>();
            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(schemeId)).Returns(scheme);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            SchemeData schemeData = A.Fake<SchemeData>();
            A.CallTo(() => schemeMap.Map(scheme)).Returns(schemeData);

            GetSchemeByIdHandler handler = new GetSchemeByIdHandler(dataAccess, schemeMap, authorization);

            GetSchemeById request = new GetSchemeById(schemeId);

            // Act
            SchemeData result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(schemeData, result);
        }

        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to get scheme data.
        /// </summary>
        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void GetSchemeByIdHandler_WithNonInternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrage
            Guid schemeId = new Guid("AC9116BC-5732-4F80-9AED-A6E2A0C4C1F1");

            IGetSchemeByIdDataAccess dataAccess = A.Fake<IGetSchemeByIdDataAccess>();
            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(schemeId)).Returns(scheme);
            
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            
            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            SchemeData schemeData = A.Fake<SchemeData>();
            A.CallTo(() => schemeMap.Map(scheme)).Returns(schemeData);

            GetSchemeByIdHandler handler = new GetSchemeByIdHandler(dataAccess, schemeMap, authorization);

            GetSchemeById request = new GetSchemeById(schemeId);

            // Act
            Func<Task<SchemeData>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        /// <summary>
        /// This test ensures that an ArgumentException is thrown if the scheme ID
        /// supplied cannot be found.
        /// </summary>
        [Fact]
        public async void GetSchemeByIdHandler_WithUnknownId_ThrowsArgumentException()
        {
            // Arrage
            Guid badSchemeId = new Guid("88C60FAC-1172-43F2-9AA5-7E79A8877F92");

            IGetSchemeByIdDataAccess dataAccess = A.Fake<IGetSchemeByIdDataAccess>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(badSchemeId)).Returns((Scheme)null);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();

            GetSchemeByIdHandler handler = new GetSchemeByIdHandler(dataAccess, schemeMap, authorization);

            GetSchemeById request = new GetSchemeById(badSchemeId);

            // Act
            Func<Task<SchemeData>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
