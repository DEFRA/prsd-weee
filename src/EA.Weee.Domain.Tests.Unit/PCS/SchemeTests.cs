namespace EA.Weee.Domain.Tests.Unit.PCS
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
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
            var producer = GetTestProducer("WEE/12345678");
            producer.MemberUpload.Submit();
            scheme.SetProducers(new List<Producer> { producer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var producers = scheme.GetProducersList(complianceYear);

            Assert.NotNull(producers);
            Assert.Equal(1, producers.Count);
        }

        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducers_ReturnsSpecifiedComplianceYearProducers()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer("WEE/12345678");
            var anotherProducer = GetTestProducer("WEE/87654321");

            producer.MemberUpload.Submit();
            anotherProducer.MemberUpload.Submit();

            scheme.SetProducers(new List<Producer> { producer, anotherProducer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var producers = scheme.GetProducersList(complianceYear);

            Assert.NotNull(producers);
            Assert.Equal(2, producers.Count);
            foreach (var item in producers)
            {
                Assert.Equal(item.MemberUpload.ComplianceYear, complianceYear);
            }
        }

        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducers_ReturnsOnlySubmittedProducers()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer("WEE/12345678");
            var anotherProducer = GetTestProducer("WEE/87654321");
            var oneAnotherProducer = GetTestProducer("WEE/54545454");

            producer.MemberUpload.Submit();
            anotherProducer.MemberUpload.Submit();

            scheme.SetProducers(new List<Producer> { producer, anotherProducer, oneAnotherProducer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var producers = scheme.GetProducersList(complianceYear);

            Assert.NotNull(producers);
            Assert.Equal(2, producers.Count);
            foreach (var item in producers)
            {
                Assert.True(item.MemberUpload.IsSubmitted);
            }
        }

        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducersWithSamePRN_ReturnsOnlyLatestProducer()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer("WEE/12345678");
            var anotherProducer = GetTestProducer("WEE/12345678");

            producer.MemberUpload.Submit();
            anotherProducer.MemberUpload.Submit();

            scheme.SetProducers(new List<Producer> { producer, anotherProducer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var producers = scheme.GetProducersList(complianceYear);

            Assert.NotNull(producers);
            Assert.Equal(1, producers.Count);
        }

        [Fact]
        public void GetProducerCSVByComplianceYear_SchemeHasProducers_ReturnsProducersCSVstring()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer("WEE/12345678");
            producer.MemberUpload.Submit();
            producer.MemberUpload.SetProducers(new List<Producer> { producer });
            scheme.SetProducers(new List<Producer> { producer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var csvData = scheme.GetProducerCSV(complianceYear);

            Assert.NotNull(csvData);
            Assert.True(csvData.Contains("WEE/12345678"));
        }

        [Fact]
        public void GetProducerCSVByComplianceYear_SchemeHasProducerWithCompanyAndAuthorisedRepresentativeNull_ReturnsProducersCSVWithCorrectFieldValues()
        {
            var scheme = GetTestScheme();
            var producer = GetTestProducer("WEE/12345678", "Test trading name", null, null);
            producer.MemberUpload.Submit();
            producer.MemberUpload.SetProducers(new List<Producer> { producer });
            scheme.SetProducers(new List<Producer> { producer });
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var csvData = scheme.GetProducerCSV(complianceYear);

            var csvFieldValues = ReadCSVLine(csvData, 1);

            Assert.NotNull(csvData);
            Assert.Equal(csvFieldValues[0], "Test trading name");
            Assert.Equal(csvFieldValues[1], "WEE/12345678");
            Assert.Equal(csvFieldValues[2], String.Empty);
            Assert.Equal(csvFieldValues[4], scheme.Producers.First().LastSubmitted.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(csvFieldValues[5], "No");
            Assert.Equal(csvFieldValues[6], String.Empty);
        }

        private string[] ReadCSVLine(string csvData, int lineNumbeer)
        {
            var csvLines = Regex.Split(csvData, "\r\n");
            var csvFieldValues = csvLines[lineNumbeer].Split(',');

            return csvFieldValues;
        }

        private static Scheme GetTestScheme()
        {
            var orgId = new Guid(orgGuid);
            var scheme = new Scheme(orgId);
            return scheme;
        }

        private static Producer GetTestProducer(string prn)
        {
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

            var producer = new Producer(Guid.NewGuid(), memberUpload, business, authorisedRepresentative, DateTime.Now, 1000000000, true,
                prn, DateTime.Now.AddDays(10), "Trading name", EEEPlacedOnMarketBandType.Both, SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>());

            return producer;
        }

        private static Producer GetTestProducer(string prn, string tradingName, Company companyDetails, AuthorisedRepresentative authorisedRepresentative)
        {
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>());
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var partnership = new Partnership("Name", producerContact, new List<Partner>());
            var business = new ProducerBusiness(companyDetails, partnership, producerContact);

            var producer = new Producer(Guid.NewGuid(), memberUpload, business, authorisedRepresentative, DateTime.Now, 1000000000, true,
                prn, DateTime.Now.AddDays(10), tradingName, EEEPlacedOnMarketBandType.Both, SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>());

            return producer;
        }
    }
}
