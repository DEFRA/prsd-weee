namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Tests.Unit.Helpers;
    using EA.Weee.Requests.Scheme;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
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
            Domain.Scheme.Scheme scheme = A.Fake<Domain.Scheme.Scheme>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(schemeId)).Returns(scheme);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var schemeMap = A.Fake<IMap<EA.Weee.Domain.Scheme.Scheme, SchemeData>>();
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
            Domain.Scheme.Scheme scheme = A.Fake<Domain.Scheme.Scheme>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(schemeId)).Returns(scheme);
            
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            
            var schemeMap = A.Fake<IMap<EA.Weee.Domain.Scheme.Scheme, SchemeData>>();
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
            A.CallTo(() => dataAccess.GetSchemeOrDefault(badSchemeId)).Returns((Domain.Scheme.Scheme)null);

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            var schemeMap = A.Fake<IMap<EA.Weee.Domain.Scheme.Scheme, SchemeData>>();

            GetSchemeByIdHandler handler = new GetSchemeByIdHandler(dataAccess, schemeMap, authorization);

            GetSchemeById request = new GetSchemeById(badSchemeId);

            // Act
            Func<Task<SchemeData>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
