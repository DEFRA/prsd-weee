namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using Weee.Domain;
    using Weee.Domain.Producer;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer;
    using Xunit;

    public class ProducerQuerySetTests
    {
        private readonly ICurrentCompanyProducers currentCompanyProducers;
        private readonly IExistingProducerNames existingProducerNames;
        private readonly IExistingProducerRegistrationNumbers existingProducerRegistrationNumbers;
        private readonly ICurrentProducersByRegistrationNumber currentProducersByRegistrationNumber;

        public ProducerQuerySetTests()
        {
            currentCompanyProducers = A.Fake<ICurrentCompanyProducers>();
            existingProducerNames = A.Fake<IExistingProducerNames>();
            existingProducerRegistrationNumbers = A.Fake<IExistingProducerRegistrationNumbers>();
            currentProducersByRegistrationNumber = A.Fake<ICurrentProducersByRegistrationNumber>();
        }

        [Fact]
        public void GetLatestProducerForComplianceYearAndScheme_AllParametersMatch_ReturnsProducer()
        {
            var schemeOrganisationId = Guid.NewGuid();
            var registrationNumber = "ABC12345";
            var complianceYear = 2016;

            var producer = FakeProducer.Create(ObligationType.Both, registrationNumber, true, schemeOrganisationId, complianceYear);

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { registrationNumber, new List<Producer> { producer }}
                });

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

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { registrationNumber, new List<Producer> { producer }}
                });

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

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { existingPrn, new List<Producer> { producer }}
                });

            var result = ProducerQuerySet().GetLatestProducerForComplianceYearAndScheme(thisPrn, complianceYear.ToString(), schemeOrganisationId);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("ABC12345", "ABC12346")]
        [InlineData("ABC12346", "ABC12345")]
        public void GetLatestProducerFromPreviousComplianceYears_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
        {
            var producer = FakeProducer.Create(ObligationType.Both, existingPrn, true, Guid.NewGuid(), 2016);

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { existingPrn, new List<Producer> { producer }}
                });

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(thisPrn);

            Assert.Null(result);
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesInConsecutiveYears_ReturnsLatestProducerByComplianceYear()
        {
            const string prn = "ABC12345";

            var oldestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2014);
            var newestProducer = FakeProducer.Create(ObligationType.Both, prn, true, Guid.NewGuid(), 2015);

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { oldestProducer, newestProducer }}
                });

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

            Assert.Equal(newestProducer, result);
        }

        [Fact]
        public void GetLatestProducerFromCurrentComplianceYearForAnotherSchemeSameObligationType_ReturnsAnotherSchemeProducer()
        {
            const string prn = "ABC12345";
            Guid schemeOrgId = Guid.NewGuid();
            var anotherSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, schemeOrgId, 2016);
            var currentSchemeProducer = FakeProducer.Create(ObligationType.B2B, prn, true, Guid.NewGuid(), 2016);

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { anotherSchemeProducer, currentSchemeProducer }}
                });

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

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { oldestProducer, newestProducer }},
                });

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

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { producer2014, oldestProducer2015, newestProducer2015 }}
                });

            var result = ProducerQuerySet().GetLatestProducerFromPreviousComplianceYears(prn);

            Assert.Equal(newestProducer2015, result);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2C)]
        [InlineData(ObligationType.B2C, ObligationType.B2B)]
        public void
            GetProducerForOtherSchemeAndObligationType_ProducerForAnotherSchemeHasDifferentObligationType_ReturnsNull(
            ObligationType existingProducerObligationType, ObligationType obligationType)
        {
            const string prn = "ABC12345";
            var schemeOrgansationId = Guid.NewGuid();

            var producer = FakeProducer.Create(existingProducerObligationType, prn, Guid.NewGuid());

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { producer } }
                });

            var result = ProducerQuerySet()
                .GetProducerForOtherSchemeAndObligationType(prn, "2016", schemeOrgansationId, (int)obligationType);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.B2C)]
        [InlineData(ObligationType.Both, ObligationType.B2B)]
        [InlineData(ObligationType.Both, ObligationType.B2C)]
        [InlineData(ObligationType.B2B, ObligationType.Both)]
        [InlineData(ObligationType.B2C, ObligationType.Both)]
        public void
            GetProducerForOtherSchemeAndObligationType_ProducerForAnotherSchemeHasMatchingObligationType_ReturnsProducer(
            ObligationType existingProducerObligationType, ObligationType obligationType)
        {
            const string prn = "ABC12345";
            var schemeOrgansationId = Guid.NewGuid();

            var producer = FakeProducer.Create(existingProducerObligationType, prn, Guid.NewGuid());

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { producer } }
                });

            var result = ProducerQuerySet()
                .GetProducerForOtherSchemeAndObligationType(prn, "2016", schemeOrgansationId, (int)obligationType);

            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(ObligationType.B2B)]
        [InlineData(ObligationType.B2C)]
        [InlineData(ObligationType.Both)]
        public void
            GetProducerForOtherSchemeAndObligationType_ProducerExistsInOtherSchemesWithB2bAndB2cObligationTypes_ReturnsProducer(ObligationType obligationType)
        {
            const string prn = "ABC12345";

            var producer1 = FakeProducer.Create(ObligationType.B2B, prn, Guid.NewGuid());
            var producer2 = FakeProducer.Create(ObligationType.B2C, prn, Guid.NewGuid());

            A.CallTo(() => currentProducersByRegistrationNumber.Run())
                .Returns(new Dictionary<string, List<Producer>>
                {
                    { prn, new List<Producer> { producer1, producer2 } }
                });

            var result = ProducerQuerySet()
                .GetProducerForOtherSchemeAndObligationType(prn, "2016", Guid.NewGuid(), (int)obligationType);

            Assert.NotNull(result);
        }

        private ProducerQuerySet ProducerQuerySet()
        {
            return new ProducerQuerySet(currentProducersByRegistrationNumber, existingProducerNames, existingProducerRegistrationNumbers, currentCompanyProducers);
        }
    }
}
