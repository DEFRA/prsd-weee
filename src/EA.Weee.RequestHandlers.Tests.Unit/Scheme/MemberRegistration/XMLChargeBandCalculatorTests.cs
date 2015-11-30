namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.Xml.Schemas;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;
    using Xunit;

    public class XmlChargeBandCalculatorTests
    {
        private readonly IProducerChargeCalculator producerChargerCalculator;
        public XmlChargeBandCalculatorTests()
        {
            producerChargerCalculator = A.Fake<IProducerChargeCalculator>();
        }

        [Fact]
        public void Calculate_WithValidXml_GeneratesNoErrors()
        {
            // Arrange

            // This file contains no errors.
            string absoluteFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-valid.xml");

            byte[] xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));
            ProcessXMLFile request = new ProcessXMLFile(A<Guid>._, xml, "File name");

            var xmlChargeBandCalculator = XmlChargeBandCalculator();

            // Act
            XmlChargeBandCalculator().Calculate(request);

            // Assert
            Assert.Empty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void Calculate_XmlWithSameProducerName_AddsError()
        {
            // Arrange

            // This file contains no errors.
            string absoluteFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-same-producer-name.xml");

            byte[] xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));
            ProcessXMLFile request = new ProcessXMLFile(A<Guid>._, xml, "File name");

            var xmlChargeBandCalculator = XmlChargeBandCalculator();

            // Act
            xmlChargeBandCalculator.Calculate(request);

            // Assert
            Assert.NotEmpty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void Calculate_WithVaildXmlFileContainingFiveProducers_ReturnsFiveCharges()
        {
            // Arrange

            // This file contains 5 producers.
            string absoluteFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-valid-ChargeBand.xml");

            byte[] xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));
            ProcessXMLFile request = new ProcessXMLFile(A<Guid>._, xml, "File name");

            ProducerCharge producerCharge1 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge2 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge3 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge4 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge5 = A.Dummy<ProducerCharge>();

            A.CallTo(() => producerChargerCalculator.CalculateCharge(A<string>._, A<producerType>._, A<int>._))
                .ReturnsNextFromSequence(producerCharge1, producerCharge2, producerCharge3, producerCharge4, producerCharge5);

            var xmlChargeBandCalculator = XmlChargeBandCalculator();

            // Act
            var producerCharges = xmlChargeBandCalculator.Calculate(request);

            // Assert
            Assert.NotNull(producerCharges);
            Assert.Equal(producerCharges.Count, 5);
            Assert.True(producerCharges.ContainsKey("The Empire"));
            Assert.True(producerCharges.ContainsKey("Tom and Jerry"));
            Assert.True(producerCharges.ContainsKey("The Empire 1"));
            Assert.True(producerCharges.ContainsKey("The Empire 2"));
            Assert.True(producerCharges.ContainsKey("The Empire 3"));

            Assert.Equal(producerCharge1, producerCharges["The Empire"]);
            Assert.Equal(producerCharge2, producerCharges["Tom and Jerry"]);
            Assert.Equal(producerCharge3, producerCharges["The Empire 1"]);
            Assert.Equal(producerCharge4, producerCharges["The Empire 2"]);
            Assert.Equal(producerCharge5, producerCharges["The Empire 3"]);
        }

        private XmlChargeBandCalculator XmlChargeBandCalculator()
        {
            var xmlConverter = new XmlConverter(A.Fake<IWhiteSpaceCollapser>(), new Deserializer());

            return new XmlChargeBandCalculator(xmlConverter, producerChargerCalculator);
        }
    }
}
