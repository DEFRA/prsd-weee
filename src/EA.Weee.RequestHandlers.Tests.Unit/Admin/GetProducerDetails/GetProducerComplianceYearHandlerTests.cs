namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetProducerComplianceYearHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External)]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            IGetProducerComplianceYearDataAccess dataAccess = A.Dummy<IGetProducerComplianceYearDataAccess>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            GetProducerComplianceYearHandler handler = new GetProducerComplianceYearHandler(dataAccess, authorization);

            Requests.Admin.GetProducerComplianceYear request = new Requests.Admin.GetProducerComplianceYear()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            Func<Task<List<int>>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownRegistrationNumber_ThrowsArgumentException()
        {
            // Arrange
            IGetProducerComplianceYearDataAccess dataAccess = A.Fake<IGetProducerComplianceYearDataAccess>();
            A.CallTo(() => dataAccess.GetComplianceYears("WEE/AA1111AA"))
                .Returns(new List<int>());

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerComplianceYearHandler handler = new GetProducerComplianceYearHandler(dataAccess, authorization);

            Requests.Admin.GetProducerComplianceYear request = new Requests.Admin.GetProducerComplianceYear()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            Func<Task<List<int>>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsComplianceYearsForProducer()
        {
            // Arrange
            IGetProducerComplianceYearDataAccess dataAccess = A.Fake<IGetProducerComplianceYearDataAccess>();
            A.CallTo(() => dataAccess.GetComplianceYears("WEE/AA1111AA"))
                .Returns(new List<int> { 2018, 2016 });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerComplianceYearHandler handler = new GetProducerComplianceYearHandler(dataAccess, authorization);

            Requests.Admin.GetProducerComplianceYear request = new Requests.Admin.GetProducerComplianceYear()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Collection(result,
                r1 => Assert.Equal(2018, r1),
                r2 => Assert.Equal(2016, r2));
        }
    }
}
