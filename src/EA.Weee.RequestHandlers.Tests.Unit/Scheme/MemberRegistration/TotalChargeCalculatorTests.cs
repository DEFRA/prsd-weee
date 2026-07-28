namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using Domain;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using EA.Weee.Xml.Deserialization;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Xml.Converter;
    using Xml.MemberRegistration;
    using Xunit;

    public class TotalChargeCalculatorTests
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly TotalChargeCalculator totalChargeCalculator;
        private readonly IAnnualChargeDataAccess annualChargeDataAccess;
        private readonly IXmlConverter xmlConverter;

        private decimal? totalCharge;
        private readonly ProcessXmlFile file;

        public TotalChargeCalculatorTests()
        {
            xmlChargeBandCalculator = A.Fake<IXMLChargeBandCalculator>();
            xmlConverter = A.Fake<IXmlConverter>();
            annualChargeDataAccess = A.Fake<IAnnualChargeDataAccess>();

            totalCharge = 0;
            file = ProcessTestXmlFile();

            totalChargeCalculator = new TotalChargeCalculator(xmlChargeBandCalculator, xmlConverter, annualChargeDataAccess);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenXMLFile_CalculateXMLChargeBand()
        {
            totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, A.Dummy<bool>(), ref totalCharge);

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeHasAnnualChargeForComplianceYear_TotalShouldNotContainAnnualCharge()
        {
            var scheme = Scheme();
            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2019, false, ref totalCharge);

            Assert.Equal(totalCharge, 0);
        }

        [Theory]
        [InlineData(2019, 12500.00)]
        [InlineData(2020, 12500.00)]
        [InlineData(2021, 12500.00)]
        [InlineData(2022, 12500.00)]
        [InlineData(2023, 12500.00)]
        [InlineData(2024, 12500.00)]
        [InlineData(2025, 12500.00)]
        public void TotalCalculatedCharges_EAScheme_2019To2025_AppliesCorrectAnnualCharge(int complianceYear, decimal expectedAnnualCharge)
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var competentAuthority = new UKCompetentAuthority(
                competentAuthorityId,
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                12500.00m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, complianceYear))
                .Returns(expectedAnnualCharge);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, complianceYear, true, ref totalCharge);

            // Assert
            Assert.Equal(300 + expectedAnnualCharge, totalCharge);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, complianceYear))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(2026, 13438.00)]
        [InlineData(2027, 13948.13)]
        [InlineData(2028, 13948.13)]
        public void TotalCalculatedCharges_EAScheme_2026AndLater_AppliesUpliftedAnnualCharge(int complianceYear, decimal expectedAnnualCharge)
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var competentAuthority = new UKCompetentAuthority(
                competentAuthorityId,
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                expectedAnnualCharge);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, complianceYear))
                .Returns(expectedAnnualCharge);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, complianceYear, true, ref totalCharge);

            // Assert
            Assert.Equal(300 + expectedAnnualCharge, totalCharge);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, complianceYear))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("SEPA", 2026)]
        [InlineData("NRW", 2026)]
        [InlineData("NIEA", 2026)]
        [InlineData("SEPA", 2027)]
        [InlineData("NRW", 2027)]
        [InlineData("NIEA", 2027)]
        public void TotalCalculatedCharges_NonEAScheme_DoesNotApplyAnnualCharge(string abbreviation, int complianceYear)
        {
            // Arrange
            var competentAuthority = new UKCompetentAuthority(
                Guid.NewGuid(),
                "Test Authority",
                abbreviation,
                A.Fake<Country>(),
                "test@authority.gov.uk",
                0m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, complianceYear, true, ref totalCharge);

            // Assert
            Assert.Equal(300, totalCharge); // Only producer charges, no annual charge
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(A<Guid>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void TotalCalculatedCharges_EAScheme_2018OrEarlier_DoesNotApplyAnnualCharge()
        {
            // Arrange
            var competentAuthority = new UKCompetentAuthority(
                Guid.NewGuid(),
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                12500.00m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2018, true, ref totalCharge);

            // Assert
            Assert.Equal(300, totalCharge); // Only producer charges
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(A<Guid>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void TotalCalculatedCharges_EAScheme_AnnualChargeNotToBeAdded_DoesNotApplyAnnualCharge()
        {
            // Arrange
            var competentAuthority = new UKCompetentAuthority(
                Guid.NewGuid(),
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                12500.00m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2026, false, ref totalCharge);

            // Assert
            Assert.Equal(300, totalCharge); // Only producer charges
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(A<Guid>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void TotalCalculatedCharges_EAScheme_AnnualChargeReturnsNull_AppliesOnlyProducerCharges()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var competentAuthority = new UKCompetentAuthority(
                competentAuthorityId,
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                12500.00m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026))
                .Returns((decimal?)null);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2026, true, ref totalCharge);

            // Assert
            Assert.Equal(300, totalCharge); // Only producer charges, annual charge was null
        }

        [Fact]
        public void TotalCalculatedCharges_EAScheme_2027_AppliesUpliftedAnnualCharge_13948_13()
        {
            // Arrange - Specific test for the 3.8% inflation uplift from April 2026
            var competentAuthorityId = Guid.NewGuid();
            var competentAuthority = new UKCompetentAuthority(
                competentAuthorityId,
                "Environment Agency",
                "EA",
                A.Fake<Country>(),
                "test@ea.gov.uk",
                13948.13m);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            var producerCharges = ProducerCharges();
            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2027))
                .Returns(13948.13m);

            totalCharge = 0;

            // Act
            totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2027, true, ref totalCharge);

            // Assert
            Assert.Equal(300 + 13948.13m, totalCharge);
            Assert.Equal(14248.13m, totalCharge);
            A.CallTo(() => annualChargeDataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2027))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerChargesAndNoAnnualCharge_TotalShouldBeCalculated()
        {
            var producerCharges = ProducerCharges();

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            var result = totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, false, ref totalCharge);

            Assert.Equal(300, totalCharge);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerCharges_ProducerChargesShouldBeReturned()
        {
            var producerCharges = ProducerCharges();

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            var result = totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, false, ref totalCharge);

            Assert.Equal(producerCharges, result);
        }

        private Scheme Scheme()
        {
            var scheme = A.Fake<Scheme>();
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            A.CallTo(() => competentAuthority.AnnualChargeAmount).Returns(100);
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);
            return scheme;
        }

        private static ProcessXmlFile ProcessTestXmlFile()
        {
            string absoluteFilePath = Path.Combine(
               Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
               @"ExampleXML\v3-valid-ChargeBand.xml");
            byte[] xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));
            ProcessXmlFile request = new ProcessXmlFile(A.Dummy<Guid>(), xml, "File name");
            return request;
        }

        private Dictionary<string, ProducerCharge> ProducerCharges()
        {
            var producerCharges = new Dictionary<string, ProducerCharge>
            {
                { "1", new ProducerCharge() { Amount = 100 } },
                { "2", new ProducerCharge() { Amount = 200 } }
            };
            return producerCharges;
        }
    }
}