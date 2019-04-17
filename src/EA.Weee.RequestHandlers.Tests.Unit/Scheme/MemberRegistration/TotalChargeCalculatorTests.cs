namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using Domain;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using EA.Weee.Xml.Deserialization;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
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
        private readonly IXmlConverter xmlConverter;

        private decimal? totalCharge;
        private readonly ProcessXmlFile file;

        public TotalChargeCalculatorTests()
        {
            xmlChargeBandCalculator = A.Fake<IXMLChargeBandCalculator>();
            xmlConverter = A.Fake<IXmlConverter>();

            totalCharge = 0;
            file = ProcessTestXmlFile();

            totalChargeCalculator = new TotalChargeCalculator(xmlChargeBandCalculator, xmlConverter);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenXMLFile_CalculateXMLChargeBand()
        {
            totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, A.Dummy<bool>(), ref totalCharge);

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeHasAnnualChargeForComplianceYear_TotalShouldNotContainAnnualCharge()
        {
            var scheme = Scheme();
            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2019, A.Dummy<bool>(), ref totalCharge);

            Assert.Equal(totalCharge, 0);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeDoesNotHaveAnnualChargeForComplianceYear_TotalShouldContainAnnualCharge()
        {
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), A.Dummy<string>(), "EA", A.Dummy<Country>(), A.Dummy<string>(), 100);

            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(competentAuthority);

            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2019, true, ref totalCharge);

            Assert.Equal(totalCharge, 100);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerChargesAndNoAnnualCharge_TotalShouldBeCalculated()
        {
            var producerCharges = ProducerCharges();

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            var result = totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, true, ref totalCharge);

            Assert.Equal(300, totalCharge);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerChargesAndAnnualCharge_TotalShouldBeCalculated()
        {
            var producerCharges = ProducerCharges();

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            var result = totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, true, ref totalCharge);

            Assert.Equal(300, totalCharge);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerCharges_ProducerChargesShouldBeReturned()
        {
            var producerCharges = ProducerCharges();

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).Returns(producerCharges);

            var result = totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, A.Dummy<bool>(), ref totalCharge);
            
            Assert.Equal(producerCharges, result);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeDoesNotHaveAnnualChargeForComplianceYearAndBefore2019_TotalShouldNotContainAnnualCharge()
        {
            var scheme = Scheme();
            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2018, false, ref totalCharge);

            Assert.Equal(totalCharge, 0);
        }

        private Scheme Scheme()
        {
            var scheme = A.Fake<Scheme>();
            var competantAuthority = A.Fake<UKCompetentAuthority>();
            A.CallTo(() => competantAuthority.AnnualChargeAmount).Returns(100);
            A.CallTo(() => scheme.CompetentAuthority).Returns(competantAuthority);
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

        private XmlChargeBandCalculator XmlChargeBandCalculator()
        {
            var xmlConverter = new XmlConverter(A.Fake<IWhiteSpaceCollapser>(), new Deserializer());

            IProducerChargeBandCalculatorChooser producerChargerCalculator = A.Fake<IProducerChargeBandCalculatorChooser>();
            return new XmlChargeBandCalculator(xmlConverter, producerChargerCalculator);
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