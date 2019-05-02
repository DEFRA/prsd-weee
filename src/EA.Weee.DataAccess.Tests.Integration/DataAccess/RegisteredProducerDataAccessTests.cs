namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using Weee.Tests.Core.Model;
    using FakeItEasy;
    using Weee.DataAccess.DataAccess;
    using Xunit;

    public class RegisteredProducerDataAccessTests
    {
        public async void GetProducerRegistration_GivenProducerRegistration_ReturnsProducerRegistration()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var modelHelper = new ModelHelper(databaseWrapper.Model);

                var organisation = modelHelper.CreateOrganisation();
                var scheme = modelHelper.CreateScheme(organisation);
                var registeredProducer = modelHelper.GetOrCreateRegisteredProducer(scheme, 2019, "registrationNumber");
                //var producerSubmission = new ProducerSubmission();
                //producerSubmission.RegisteredProducer = registeredProducer;
                //databaseWrapper.Model.ProducerSubmissions.Add(producerSubmission);

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
