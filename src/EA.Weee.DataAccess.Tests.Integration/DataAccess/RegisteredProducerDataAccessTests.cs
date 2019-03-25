namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using FakeItEasy;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class RegisteredProducerDataAccessTests
    {
        [Fact]
        public async void GetProducerRegistration_GivenProducerRegistration_ReturnsProducerRegistration()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var organisation = modelHelper.CreateOrganisation();
                var scheme = modelHelper.CreateScheme(organisation);
                var registeredProducer = modelHelper.GetOrCreateRegisteredProducer(scheme, 2019, "registrationNumber");

                await databaseWrapper.Model.SaveChangesAsync();

                // Act
                var dataAccess = new RegisteredProducerDataAccess(databaseWrapper.WeeeContext);

                var result = await dataAccess.GetProducerRegistration(registeredProducer.ProducerRegistrationNumber, 2019, scheme.ApprovalNumber);

                // Assert
                Assert.Equal(registeredProducer.Id, result.Id);
            }
        }
    }
}
