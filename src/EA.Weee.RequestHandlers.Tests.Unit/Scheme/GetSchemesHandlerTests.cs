namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemesHandlerTests
    {
        /// <summary>
        /// This test ensures that an authorized user can execute requests to get scheme data
        /// if they provide a valid scheme ID.
        /// </summary>
        [Fact]
        public async void GetSchemesHandlerTest_HappyPath_ReturnsSchemeDatasOrderedByName()
        {
            // Arrage
            IGetSchemesDataAccess dataAccess = CreateFakeDataAccess();

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();

            IMap<Scheme, SchemeData> schemeMap = CreateFakeSchemeMap();

            GetSchemesHandler handler = new GetSchemesHandler(dataAccess, schemeMap, authorization);

            GetSchemes request = new GetSchemes();

            // Act
            List<SchemeData> results = await handler.HandleAsync(request);

            // Assert
            Assert.Collection(
                results,
                (element1) => Assert.Equal(schemeData2, element1),
                (element2) => Assert.Equal(schemeData1, element2),
                (element3) => Assert.Equal(schemeData3, element3));
        }

        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to get scheme data.
        /// </summary>
        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void GetSchemesHandler_WithNonInternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IGetSchemesDataAccess dataAccess = CreateFakeDataAccess();

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            IMap<Scheme, SchemeData> schemeMap = CreateFakeSchemeMap();

            GetSchemesHandler handler = new GetSchemesHandler(dataAccess, schemeMap, authorization);

            GetSchemes request = new GetSchemes();

            // Act
            Func<Task<List<SchemeData>>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        private Domain.Scheme.Scheme scheme1;
        private Domain.Scheme.Scheme scheme2;
        private Domain.Scheme.Scheme scheme3;

        private IGetSchemesDataAccess CreateFakeDataAccess()
        {
            IGetSchemesDataAccess dataAccess = A.Fake<IGetSchemesDataAccess>();
            
            scheme1 = A.Fake<Domain.Scheme.Scheme>();
            scheme2 = A.Fake<Domain.Scheme.Scheme>();
            scheme3 = A.Fake<Domain.Scheme.Scheme>();

            var results = new List<Domain.Scheme.Scheme>()
            { 
                scheme1,
                scheme2,
                scheme3
            };

            A.CallTo(() => dataAccess.GetCompleteSchemes()).Returns(results);
            return dataAccess;
        }

        private SchemeData schemeData1;
        private SchemeData schemeData2;
        private SchemeData schemeData3;

        private IMap<Scheme, SchemeData> CreateFakeSchemeMap()
        {
            IMap<Scheme, SchemeData> schemeMap = A.Fake<IMap<EA.Weee.Domain.Scheme.Scheme, SchemeData>>();

            schemeData1 = A.Fake<SchemeData>();
            schemeData1.Name = "MCH";
            A.CallTo(() => schemeMap.Map(scheme1)).Returns(schemeData1);

            schemeData2 = A.Fake<SchemeData>();
            schemeData2.Name = "ARB";
            A.CallTo(() => schemeMap.Map(scheme2)).Returns(schemeData2);

            schemeData3 = A.Fake<SchemeData>();
            schemeData3.Name = "ZRS";
            A.CallTo(() => schemeMap.Map(scheme3)).Returns(schemeData3);

            return schemeMap;
        }
    }
}
