namespace EA.Weee.Domain.Tests.Unit.PCS
{
    using Domain.PCS;
    using FakeItEasy;
    using Producer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
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

        /// <summary>
        /// This test ensures that only producers included in submitted member uploads
        /// for the specified year are returned by Scheme.GetProcucersList(int).
        /// </summary>
        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducers_ReturnsOnlyProducersForTheSpecifiedYear()
        {
            // Arrange
            MemberUpload memberUpload2015 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2015.ComplianceYear).Returns(2015);
            A.CallTo(() => memberUpload2015.IsSubmitted).Returns(true);

            MemberUpload memberUpload2016 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2016.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload2016.IsSubmitted).Returns(true);

            Producer producer1 = A.Fake<Producer>();
            A.CallTo(() => producer1.MemberUpload).Returns(memberUpload2015);
            A.CallTo(() => producer1.IsCurrentForComplianceYear).Returns(true);

            Producer producer2 = A.Fake<Producer>();
            A.CallTo(() => producer2.MemberUpload).Returns(memberUpload2016);
            A.CallTo(() => producer2.IsCurrentForComplianceYear).Returns(true);

            Producer producer3 = A.Fake<Producer>();
            A.CallTo(() => producer3.MemberUpload).Returns(memberUpload2016);
            A.CallTo(() => producer3.IsCurrentForComplianceYear).Returns(true);

            Guid organisationId = new Guid("5F60A794-1DB6-44E9-B66E-A5C8CBEEAF1A");

            Scheme scheme = new Scheme(organisationId);
            scheme.SetProducers(new List<Producer> { producer1, producer2, producer3 });

            // Act
            List<Producer> producers = scheme.GetProducersList(2016);

            // Assert
            Assert.NotNull(producers);
            Assert.Equal(2, producers.Count);
            Assert.False(producers.Any(item => item.MemberUpload.ComplianceYear != 2016));
        }

        /// <summary>
        /// This test ensures that only producers in member uploads which have been submitted
        /// are returned by Scheme.GetProcucersList(int).
        /// </summary>
        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducers_ReturnsOnlySubmittedProducers()
        {
            // Arrange
            MemberUpload memberUploadSubmitted = A.Fake<MemberUpload>();
            A.CallTo(() => memberUploadSubmitted.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUploadSubmitted.IsSubmitted).Returns(true);

            MemberUpload memberUploadUnsubmitted = A.Fake<MemberUpload>();
            A.CallTo(() => memberUploadUnsubmitted.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUploadUnsubmitted.IsSubmitted).Returns(false);

            Producer producer1 = A.Fake<Producer>();
            A.CallTo(() => producer1.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer1.IsCurrentForComplianceYear).Returns(true);

            Producer producer2 = A.Fake<Producer>();
            A.CallTo(() => producer2.MemberUpload).Returns(memberUploadSubmitted);
            A.CallTo(() => producer2.IsCurrentForComplianceYear).Returns(true);

            Producer producer3 = A.Fake<Producer>();
            A.CallTo(() => producer3.MemberUpload).Returns(memberUploadUnsubmitted);
            A.CallTo(() => producer3.IsCurrentForComplianceYear).Returns(false); // Must be false for an unsubmitted producer.

            Guid organisationId = new Guid("5F60A794-1DB6-44E9-B66E-A5C8CBEEAF1A");

            Scheme scheme = new Scheme(organisationId);
            scheme.SetProducers(new List<Producer> { producer1, producer2, producer3 });

            // Act
            List<Producer> producers = scheme.GetProducersList(2016);

            // Assert
            Assert.NotNull(producers);
            Assert.Equal(2, producers.Count);
            Assert.True(producers.Any(item => item.MemberUpload.IsSubmitted));
        }

        /// <summary>
        /// This test ensures that only the "current" version of a producer is returned by
        /// Scheme.GetProcucersList(int) if the producer has been included on more than one
        /// submitted member upload.
        /// </summary>
        [Fact]
        public void GetProducerListByComplianceYear_SchemeHasProducersWithSamePRN_ReturnsOnlyLatestProducer()
        {
            // Arrange
            MemberUpload memberUpload1 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload1.IsSubmitted).Returns(true);

            MemberUpload memberUpload2 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload2.IsSubmitted).Returns(true);

            Producer producer1 = A.Fake<Producer>();
            A.CallTo(() => producer1.RegistrationNumber).Returns("PRN1");
            A.CallTo(() => producer1.MemberUpload).Returns(memberUpload1);
            A.CallTo(() => producer1.IsCurrentForComplianceYear).Returns(false);

            Producer producer2 = A.Fake<Producer>();
            A.CallTo(() => producer2.RegistrationNumber).Returns("PRN1");
            A.CallTo(() => producer2.MemberUpload).Returns(memberUpload2);
            A.CallTo(() => producer2.IsCurrentForComplianceYear).Returns(true);

            Guid organisationId = new Guid("5F60A794-1DB6-44E9-B66E-A5C8CBEEAF1A");

            Scheme scheme = new Scheme(organisationId);
            scheme.SetProducers(new List<Producer> { producer1, producer2 });

            // Act
            List<Producer> producers = scheme.GetProducersList(2016);

            // Assert
            Assert.NotNull(producers);
            Assert.Equal(1, producers.Count);
            Assert.Equal(producers.Single(), producer2);
        }

        [Fact]
        public void GetProducerCSVByComplianceYear_SchemeHasProducers_ReturnsProducersCSVstring()
        {
            var scheme = GetTestScheme();

            var producer = GetTestProducer("WEE/12345678");
            producer.MemberUpload.Submit();
            producer.MemberUpload.SetProducers(new List<Producer> { producer });
            producer.SetScheme(scheme);

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

            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var partnership = new Partnership("Test partnership Name", producerContact, new List<Partner>());

            var producer = GetTestProducer("WEE/12345678", "Test trading name", null, partnership, null);
            producer.MemberUpload.Submit();
            producer.MemberUpload.SetProducers(new List<Producer> { producer });
            producer.SetScheme(scheme);

            scheme.SetProducers(new List<Producer> { producer });

            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var csvData = scheme.GetProducerCSV(complianceYear);

            var csvFieldValues = ReadCSVLine(csvData, 1);

            Assert.NotNull(csvData);
            Assert.Equal(csvFieldValues[0], "Test partnership Name");
            Assert.Equal(csvFieldValues[1], "Test trading name");
            Assert.Equal(csvFieldValues[2], "WEE/12345678");
            Assert.Equal(csvFieldValues[3], String.Empty);
            Assert.Equal(csvFieldValues[5], scheme.Producers.First().LastSubmitted.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(csvFieldValues[6], "No");
            Assert.Equal(csvFieldValues[7], string.Empty);
        }

        [Fact]
        public void GetProducerCSVByComplianceYear_SchemeHasProducerWithCompanyAndAuthorisedRepresentative_ReturnsProducersCSVWithCorrectFieldValues()
        {
            var scheme = GetTestScheme();
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var authorisedRepresentative = new AuthorisedRepresentative("Name", producerContact);
            var companyDetails = new Company("Test company name", "Test registration number", producerContact);
            
            var producer = GetTestProducer("WEE/12345678", "Test trading name", companyDetails, null, authorisedRepresentative);
            producer.MemberUpload.Submit();
            producer.MemberUpload.SetProducers(new List<Producer> { producer });
            producer.SetScheme(scheme);

            scheme.SetProducers(new List<Producer> { producer });

            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear;
            var csvData = scheme.GetProducerCSV(complianceYear);

            var csvFieldValues = ReadCSVLine(csvData, 1);

            Assert.NotNull(csvData);
            Assert.Equal(csvFieldValues[0], "Test company name");
            Assert.Equal(csvFieldValues[1], "Test trading name");
            Assert.Equal(csvFieldValues[2], "WEE/12345678");
            Assert.Equal(csvFieldValues[3], companyDetails.CompanyNumber);
            Assert.Equal(csvFieldValues[5], scheme.Producers.First().LastSubmitted.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(csvFieldValues[6], "Yes");
            Assert.Equal(csvFieldValues[7], authorisedRepresentative.OverseasProducerName);
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
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>(), 0);
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var companyDetails = new Company("Test name", "Test registration number", producerContact);
            var partnership = new Partnership("Name", producerContact, new List<Partner>());
            var business = new ProducerBusiness(companyDetails, null, producerContact);
            var authorisedRepresentative = new AuthorisedRepresentative("Name", producerContact);

            var producer = new Producer(Guid.NewGuid(), memberUpload, business, authorisedRepresentative, DateTime.Now, 1000000000, true,
                prn, DateTime.Now.AddDays(10), "Trading name", EEEPlacedOnMarketBandType.Both, SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>(), true, ChargeBandType.A);

            return producer;
        }

        private static Producer GetTestProducer(string prn, string tradingName, Company companyDetails, Partnership partnership, AuthorisedRepresentative authorisedRepresentative)
        {
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>(), 0);
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            
            var business = new ProducerBusiness(companyDetails, partnership, producerContact);

            var producer = new Producer(Guid.NewGuid(), memberUpload, business, authorisedRepresentative, DateTime.Now, 1000000000, true,
                prn, DateTime.Now.AddDays(10), tradingName, EEEPlacedOnMarketBandType.Both, SellingTechniqueType.Both, ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds, new List<BrandName>(), new List<SICCode>(), true, ChargeBandType.A);

            return producer;
        }
    }
}
