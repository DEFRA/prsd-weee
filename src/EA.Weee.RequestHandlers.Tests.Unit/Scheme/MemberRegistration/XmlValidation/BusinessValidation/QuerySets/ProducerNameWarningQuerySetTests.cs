namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
    using Xunit;

    public class ProducerNameWarningQuerySetTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public ProducerNameWarningQuerySetTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();
        }

        [Fact]
        public void GetLatestProducerForComplianceYearAndScheme_AllParametersMatch_ReturnsProducer()
        {
            var schemeOrganisationId = Guid.NewGuid();
            var registrationNumber = "ABC12345";
            var complianceYear = 2016;

            var producer = FakeProducer.Create(ObligationType.Both, registrationNumber, true, schemeOrganisationId, complianceYear);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    producer
                }));

            var result = ProducerNameWarningQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, complianceYear.ToString(), schemeOrganisationId);

            Assert.Equal(producer, result);
        }

        [Theory]
        [InlineData(2016, 2017)]
        [InlineData(2017, 2016)]
        public void GetLatestProducerForComplianceYearAndScheme_ComplianceYearDoesNotMatch_ReturnsNull(int thisComplianceYear, int existingComplianceYear)
        {
            var schemeOrganisationId = Guid.NewGuid();
            var registrationNumber = "ABC12345";

            var producer = FakeProducer.Create(ObligationType.Both, registrationNumber, true, schemeOrganisationId, existingComplianceYear);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    producer
                }));

            var result = ProducerNameWarningQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, thisComplianceYear.ToString(), schemeOrganisationId);
           
            Assert.Null(result);
        }

        [Theory]
        [InlineData("ABC12345", "ABC12346")]
        [InlineData("ABC12346", "ABC12345")]
        public void GetLatestProducerForComplianceYearAndScheme_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
        {
            var schemeOrganisationId = Guid.NewGuid();
            var complianceYear = 2016;

            var producer = FakeProducer.Create(ObligationType.Both, existingPrn, true, schemeOrganisationId, complianceYear);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    producer
                }));

            var result = ProducerNameWarningQuerySet().GetLatestProducerForComplianceYearAndScheme(thisPrn, complianceYear.ToString(), schemeOrganisationId);

            Assert.Null(result);
        }

        // TODO: Tests for the other queries

        private ProducerNameWarningQuerySet ProducerNameWarningQuerySet()
        {
            return new ProducerNameWarningQuerySet(context);
        }
    }
}
