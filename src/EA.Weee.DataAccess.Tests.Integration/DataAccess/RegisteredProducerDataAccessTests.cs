namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class RegisteredProducerDataAccessTests
    {
        [Fact]
        public async void GetDirectRegistrantByRegistration_GivenProducerRegistration_ReturnsProducerRegistration()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                DirectRegistrantHelper.SetupCommonTestData(databaseWrapper);

                var dataAccess = new RegisteredProducerDataAccess(databaseWrapper.WeeeContext);

                var (complianceYear, country) = DirectRegistrantHelper.SetupCommonTestData(databaseWrapper);
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(databaseWrapper, "My company", "WEE/AF48365JN", complianceYear);

                await DirectRegistrantHelper.CreateSubmission(databaseWrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var result =
                    await dataAccess.GetDirectRegistrantByRegistration(registeredProducer1.ProducerRegistrationNumber);

                result.Should().NotBeNull();
                result.DirectProducerSubmissions.Count.Should().Be(1);
            }
        }

        [Fact]
        public async void GetDirectRegistrantByRegistration_GivenProducerRegistrationHasBeenRemoved_ReturnsProducerRegistration()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                DirectRegistrantHelper.SetupCommonTestData(databaseWrapper);

                var dataAccess = new RegisteredProducerDataAccess(databaseWrapper.WeeeContext);

                var (complianceYear, country) = DirectRegistrantHelper.SetupCommonTestData(databaseWrapper);
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(databaseWrapper, "My company", "WEE/ZF48365JN", complianceYear);

                await DirectRegistrantHelper.CreateSubmission(databaseWrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                registeredProducer1.Remove();
                await databaseWrapper.WeeeContext.SaveChangesAsync();

                var result =
                    await dataAccess.GetDirectRegistrantByRegistration(registeredProducer1.ProducerRegistrationNumber);

                result.Should().NotBeNull();
                result.DirectProducerSubmissions.Count.Should().Be(0);
            }
        }
    }
}
