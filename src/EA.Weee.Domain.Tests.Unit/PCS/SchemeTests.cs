namespace EA.Weee.Domain.Tests.Unit.PCS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.PCS;
    using Producer;
    using Xunit;

    public class SchemeTests
    {
        private const string orgGuid = "2AE69682-E9D8-4AC5-991D-A4CF00C42F14";

        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducers_ReturnsProducers()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer();
            producer.MemberUpload.Submit();
            scheme.SetProducers(new List<Producer> { producer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var producers = scheme.GetProducersList(complianceYear);

            Assert.NotNull(producers);
            Assert.Equal(1, producers.Count);
        }

        [Fact]
        public void GetProducerCSVByComplianceYear_SchemeHasProducers_ReturnsProducersCSVstring()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer();
            producer.MemberUpload.Submit();
            scheme.SetProducers(new List<Producer> { producer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var csvData = scheme.GetProducerCSV(complianceYear);

            Assert.NotNull(csvData);
            Assert.True(csvData.Contains("WEE/12345678"));
        }

        private static Scheme GetTestScheme()
        {
            var orgId = new Guid(orgGuid);
            var scheme = new Scheme(orgId);
            return scheme;
        }

        private static Producer GetTestProducer()
        {
            var orgId = new Guid(orgGuid);
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>());
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var companyDetails = new Company("Test name", "Test registration number", producerContact);
            var partnership = new Partnership("Name", producerContact, new List<Partner>());
            var business = new ProducerBusiness(companyDetails, partnership, producerContact);
            var authorisedRepresentative = new AuthorisedRepresentative("Name", producerContact);
            var scheme = new Scheme(orgId);

            var producer = new Producer(Guid.NewGuid(), memberUpload, business, authorisedRepresentative, DateTime.Now, 1000000000, true,
                "WEE/12345678", DateTime.Now.AddDays(10), "Trading name", EEEPlacedOnMarketBandType.Both, SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>());

            return producer;
        }
    }
}
