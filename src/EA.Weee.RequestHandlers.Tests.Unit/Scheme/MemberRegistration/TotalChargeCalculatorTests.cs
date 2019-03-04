namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using Domain.Error;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Domain;
    using RequestHandlers.Security;
    using Xml.Converter;
    using Xunit;

    public class TotalChargeCalculatorTests
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly ITotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;
        private readonly TotalChargeCalculator totalChargeCalculator;
        private readonly IWeeeAuthorization authorization;
        private bool hasAnnualCharge;
        private decimal? totalCharge;
        private readonly ProcessXmlFile file;

        public TotalChargeCalculatorTests()
        {
            xmlChargeBandCalculator = A.Fake<IXMLChargeBandCalculator>();
            totalChargeCalculatorDataAccess = A.Fake<ITotalChargeCalculatorDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            file = ProcessTestXmlFile();

            totalChargeCalculator = new TotalChargeCalculator(xmlChargeBandCalculator, authorization, totalChargeCalculatorDataAccess);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenXMLFile_CalculateXMLChargeBand()
        {
            totalChargeCalculator.TotalCalculatedCharges(file, Scheme(), 2019, ref hasAnnualCharge, ref totalCharge);

            A.CallTo(() => xmlChargeBandCalculator.Calculate(file)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeHasAnnualChargeForComplianceYear_TotalShouldNotContainAnnualCharge()
        {
            var scheme = Scheme();
            A.CallTo(() => totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, 2019)).Returns(true);
            hasAnnualCharge = false;
            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2019, ref hasAnnualCharge, ref totalCharge);

            Assert.Equal(totalCharge, 0);
            Assert.Equal(hasAnnualCharge, true);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenSchemeDoesNotHaveAnnualChargeForComplianceYear_TotalShouldContainAnnualCharge()
        {
            var scheme = Scheme();
            A.CallTo(() => totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, 2019)).Returns(false);
            hasAnnualCharge = true;
            totalCharge = 0;

            var result = totalChargeCalculator.TotalCalculatedCharges(file, scheme, 2019, ref hasAnnualCharge, ref totalCharge);

            Assert.Equal(totalCharge, 100);
            Assert.Equal(hasAnnualCharge, false);
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerCharges_TotalShouldBeCalculated()
        {
        }

        [Fact]
        public void TotalCalculatedCharges_GivenProducerCharges_ProducerChargesShouldBeReturned()
        {
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
    }
}