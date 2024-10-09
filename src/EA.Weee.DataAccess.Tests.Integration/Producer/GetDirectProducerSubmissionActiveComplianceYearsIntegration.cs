namespace EA.Weee.DataAccess.Tests.Integration.Producer
{
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class GetDirectProducerSubmissionActiveComplianceYearsIntegration
    {
        [Fact]
        public async Task Get_Returns_ValidYears()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;

                DirectRegistrantHelper.SetupCommonTestData(database);

                var dataAccess = new GetDirectProducerSubmissionActiveComplianceYearsDataAccess(context);
                var (complianceYear, country) = DirectRegistrantHelper.SetupCommonTestData(database);

                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "My company", "WEE/AG48365JN", complianceYear);

                await DirectRegistrantHelper.CreateSubmission(database, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                // Should not be duplicate.
                await DirectRegistrantHelper.CreateSubmission(database, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                await DirectRegistrantHelper.CreateSubmission(database, directRegistrant1, registeredProducer1, 2090, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                // Should not be included.
                await DirectRegistrantHelper.CreateSubmission(database, directRegistrant1, registeredProducer1, complianceYear + 1, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Incomplete);

                // Act
                var results = await dataAccess.Get();

                // Assert
                results.Should().BeInDescendingOrder();
                results.Should().BeEquivalentTo(new List<int>() { complianceYear - 1, 2090 - 1 });
            }
        }
    }
}
