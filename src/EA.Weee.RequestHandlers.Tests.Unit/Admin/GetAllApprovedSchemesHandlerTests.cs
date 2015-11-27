namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{ 
    using Core.Scheme;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class GetAllApprovedSchemesHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External)]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        public async Task GetAllComplianceYearsHandler_NotAdminUser_ThrowsSecurityException(
            AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");
            var dataAccess = A.Dummy<IGetAllApprovedSchemesDataAccess>();
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            var handler = new GetAllApprovedSchemesHandler(authorization, schemeMap, dataAccess);

            var request = new GetAllApprovedSchemes();

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Asert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsAllApprovedSchemesOrderedBySchemeName()
        {
            // Arrange
            IGetAllApprovedSchemesDataAccess dataAccess = CreateFakeDataAccess();

            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .Build();
            IMap<Scheme, SchemeData> schemeMap = CreateFakeSchemeMap();

            var handler = new GetAllApprovedSchemesHandler(authorization, schemeMap, dataAccess);

            var request = new GetAllApprovedSchemes();

            // Act
            var schemesList = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(schemesList);
            Assert.Equal(2, schemesList.Count);
            Assert.Collection(schemesList,
                r1 => Assert.Equal("ARB", r1.SchemeName),
                r2 => Assert.Equal("MCH", r2.SchemeName));
        }

        private Scheme scheme1;
        private Scheme scheme2;
        private Scheme scheme3;

        private IGetAllApprovedSchemesDataAccess CreateFakeDataAccess()
        {
            var dataAccess = A.Fake<IGetAllApprovedSchemesDataAccess>();

            scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.SchemeName).Returns("MCH");
            scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.SchemeName).Returns("ARB");
            scheme3 = A.Fake<Scheme>();
            A.CallTo(() => scheme3.SchemeName).Returns("ZRS");

            var results = new List<Scheme>
            {
                scheme2,
                scheme1
            };

            A.CallTo(() => dataAccess.GetAllApprovedSchemes()).Returns(results);
            return dataAccess;
        }

        private SchemeData schemeData1;
        private SchemeData schemeData2;
        private SchemeData schemeData3;

        private IMap<Scheme, SchemeData> CreateFakeSchemeMap()
        {
            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();

            schemeData1 = A.Fake<SchemeData>();
            schemeData1.SchemeName = "MCH";
            schemeData1.SchemeStatus = SchemeStatus.Approved;
            A.CallTo(() => schemeMap.Map(scheme1)).Returns(schemeData1);

            schemeData2 = A.Fake<SchemeData>();
            schemeData2.SchemeName = "ARB";
            schemeData2.SchemeStatus = SchemeStatus.Approved;
            A.CallTo(() => schemeMap.Map(scheme2)).Returns(schemeData2);

            schemeData3 = A.Fake<SchemeData>();
            schemeData3.SchemeName = "ZRS";
            schemeData3.SchemeStatus = SchemeStatus.Pending;
            A.CallTo(() => schemeMap.Map(scheme3)).Returns(schemeData3);

            return schemeMap;
        }
    }
}
