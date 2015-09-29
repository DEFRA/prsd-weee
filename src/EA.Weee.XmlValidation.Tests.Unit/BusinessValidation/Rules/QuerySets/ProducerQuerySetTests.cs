namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using FakeItEasy;
    using Weee.Domain;
    using Weee.Domain.Producer;
    using Weee.Tests.Core;
    using XmlValidation.BusinessValidation.QuerySets;
    using Xunit;

    public class ProducerQuerySetTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public ProducerQuerySetTests()
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

            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, complianceYear.ToString(), schemeOrganisationId);

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

            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(registrationNumber, thisComplianceYear.ToString(), schemeOrganisationId);

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

            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(thisPrn, complianceYear.ToString(), schemeOrganisationId);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("ABC12345", "ABC12346")]
        [InlineData("ABC12346", "ABC12345")]
        public void GetLatestProducerFromPreviousComplianceYears_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
        {
            var producer = FakeProducer.Create(ObligationType.Both, existingPrn, true, Guid.NewGuid(), 2016);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    producer
                }));

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(thisPrn);

            Assert.Null(result);
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesInConsecutiveYears_ReturnsLatestProducerByComplianceYear()
        {
            const string prn = "ABC12345";

            var oldestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2014);
            var newestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    oldestProducer,
                    newestProducer
                }));

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

            Assert.Equal(newestProducer, result);
        }

        [Fact]
        public void GetLatestProducerFromCurrentComplianceYearForAnotherSchemeSameObligationType__ReturnsAnotherSchemeProducer()
        {
            const string prn = "ABC12345";
            Guid schemeOrgId = Guid.NewGuid();
            var anotherSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, schemeOrgId, 2016);
            var currentSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, Guid.NewGuid(), 2016);

            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    anotherSchemeProducer,
                    currentSchemeProducer
                }));

            var result = ProducerQuerySet().GetProducerForOtherSchemeAndObligationType(prn, "2016", schemeOrgId, 1);

            Assert.Equal(anotherSchemeProducer, result);
        }
        [Fact]
        public void
            GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_ReturnsLatestProducerByUploadDate()
        {
            const string prn = "ABC12345";

            var oldestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);
            var newestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    oldestProducer,
                    newestProducer
                }));

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

            Assert.Equal(newestProducer, result);
        }

        [Fact]
        public void
            GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_AndOneIn2014_ReturnsLatestProducerByUploadDateIn2015()
        {
            const string prn = "ABC12345";

            var producer2014 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2014);
            var oldestProducer2015 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);
            var newestProducer2015 = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    producer2014,
                    oldestProducer2015,
                    newestProducer2015
                }));

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

            Assert.Equal(newestProducer2015, result);
        }

        [Fact]
        public void GetAllRegistrationNumbers_ReturnsDistinctRegistrationNumbers()
        {
            const string prnA1 = "ABC12345";
            const string prnB = "BBC12345";
            const string prnC = "CBC12345";
            const string prnA2 = "ABC12345";

            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    FakeProducer.Create(ObligationType.Both, prnA1),
                    FakeProducer.Create(ObligationType.Both, prnB),
                    FakeProducer.Create(ObligationType.Both, prnC),
                    FakeProducer.Create(ObligationType.Both, prnA2),
                    FakeProducer.Create(ObligationType.Both, prnA1)
                }));

            var result = ProducerQuerySet().GetAllRegistrationNumbers();

            Assert.Equal(3, result.Count);
            Assert.Single(result, prnA1);
            Assert.Single(result, prnB);
            Assert.Single(result, prnC);
        }

        [Fact]
        public void GetLatestCompanyProducers_ReturnsCompaniesOnly()
        {
            var company1 = FakeProducer.Create(ObligationType.Both, "123", true, producerBusiness: new ProducerBusiness(companyDetails: new Company("Company1", "123456", null)));
            var company2 = FakeProducer.Create(ObligationType.Both, "123", true, producerBusiness: new ProducerBusiness(companyDetails: new Company("Company2", "123456", null)));
            var partnership1 = FakeProducer.Create(ObligationType.Both, "123", true, producerBusiness: new ProducerBusiness(partnership: new Partnership("Partnership1", null, null)));
            var partnership2 = FakeProducer.Create(ObligationType.Both, "123", true, producerBusiness: new ProducerBusiness(partnership: new Partnership("Partnership2", null, null)));

            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    company1,
                    partnership1,
                    partnership2,
                    company2
                }));

            var result = ProducerQuerySet().GetLatestCompanyProducers();

            Assert.Equal(2, result.Count);
            Assert.Contains(company1, result);
            Assert.Contains(company2, result);
        }

        [Fact]
        public void GetLatestCompanyProducers_ReturnsCurrentForComplianceYearCompaniesOnly()
        {
            var company1 = FakeProducer.Create(ObligationType.Both, "123", true, complianceYear: 2016, producerBusiness: new ProducerBusiness(companyDetails: new Company("Company1", "123456", null)));
            var company2 = FakeProducer.Create(ObligationType.Both, "123", false, complianceYear: 2016, producerBusiness: new ProducerBusiness(companyDetails: new Company("Company2", "123456", null)));
            var company3 = FakeProducer.Create(ObligationType.Both, "123", true, complianceYear: 2017, producerBusiness: new ProducerBusiness(companyDetails: new Company("Company1", "123456", null)));

            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
                {
                    company1,
                    company2,
                    company3
                }));

            var result = ProducerQuerySet().GetLatestCompanyProducers();

            Assert.Equal(2, result.Count);
            Assert.Contains(company1, result);
            Assert.Contains(company3, result);
        }

        private ProducerQuerySet ProducerQuerySet()
        {
            return new ProducerQuerySet(context);
        }
    }
}
