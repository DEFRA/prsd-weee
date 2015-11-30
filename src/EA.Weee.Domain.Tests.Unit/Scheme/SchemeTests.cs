namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Domain.Scheme;
    using EA.Weee.Domain.Producer;
    using FakeItEasy;
    using Lookup;
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
            var complianceYear = scheme.Producers.First().MemberUpload.ComplianceYear.Value;
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
            Assert.True(ReferenceEquals(producers.Single(), producer2));
        }

        [Fact]
        public void UpdateSchemeInformation_SchemeWithBasicInformation_ReturnsUpdatedSchemeInformation()
        {
            var scheme = GetTestScheme();
            const string schemeName = "WEE/AB1234CD/SCH";
            const string approvalNumber = "Approval number";
            const string ibisCustomerReference = "Any value";
            var obligationType = ObligationType.B2B;
            var competentAuthorityId = Guid.NewGuid();

            scheme.UpdateScheme(schemeName, approvalNumber, ibisCustomerReference, obligationType, competentAuthorityId);

            Assert.Equal(scheme.SchemeName, schemeName);
            Assert.Equal(scheme.ApprovalNumber, approvalNumber);
            Assert.Equal(scheme.IbisCustomerReference, ibisCustomerReference);
            Assert.Equal(scheme.ObligationType, obligationType);
            Assert.Equal(scheme.CompetentAuthorityId, competentAuthorityId);
        }

        [Fact]
        public void UpdateSchemeStatus_ChangeFromPending_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);

            scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);

            scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Pending);
        }

        [Fact]
        public void UpdateSchemeStatus_NoChangeOfApprovedStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);
            scheme.SetStatus(SchemeStatus.Approved);
        }

        [Fact]
        public void UpdateSchemeStatus_NoChangeOfRejectedStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);
            scheme.SetStatus(SchemeStatus.Rejected);
        }

        [Fact]
        public void UpdateSchemeStatus_ApprovedToSomethingElse_ThrowsInvalidOperationException()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);

            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Pending));
            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Rejected));
        }

        [Fact]
        public void UpdateSchemeStatus_RejectedToSomethingElse_ThrowsInvalidOperationException()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);

            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Pending));
            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Approved));
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
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);
            var companyDetails = new Company("Test name", "Test registration number", producerContact);
            var partnership = new Partnership("Name", producerContact, new List<Partner>());
            var business = new ProducerBusiness(companyDetails, null, producerContact);
            var authorisedRepresentative = new AuthorisedRepresentative("Name", producerContact);

            var producer = new Producer(
                Guid.NewGuid(),
                memberUpload,
                business,
                authorisedRepresentative,
                DateTime.Now,
                1000000000,
                true,
                prn,
                DateTime.Now.AddDays(10),
                "Trading name",
                EEEPlacedOnMarketBandType.Both,
                SellingTechniqueType.Both,
                ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                true,
                A.Dummy<ChargeBandAmount>(),
                (decimal)5.0);

            return producer;
        }

        private static Producer GetTestProducer(string prn, string tradingName, Company companyDetails, Partnership partnership, AuthorisedRepresentative authorisedRepresentative, ChargeBandAmount chargeBandAmount)
        {
            var memberUpload = new MemberUpload(Guid.NewGuid(), "Test Data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");
            var country = new Country(Guid.NewGuid(), "Country name");
            var producerAddress = new ProducerAddress("Primary name", "Secondary name", "Street", "Town", "Locality",
                "Administrative area", country, "Postcode");
            var producerContact = new ProducerContact("Mr.", "Firstname", "Lastname", "12345", "9898988", "43434433",
                "test@test.com", producerAddress);

            var business = new ProducerBusiness(companyDetails, partnership, producerContact);

            var producer = new Producer(
                Guid.NewGuid(),
                memberUpload,
                business,
                authorisedRepresentative,
                DateTime.Now,
                1000000000,
                true,
                prn,
                DateTime.Now.AddDays(10),
                tradingName,
                EEEPlacedOnMarketBandType.Both,
                SellingTechniqueType.Both,
                ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                true,
                chargeBandAmount,
                (decimal)5.0);

            return producer;
        }
    }
}
