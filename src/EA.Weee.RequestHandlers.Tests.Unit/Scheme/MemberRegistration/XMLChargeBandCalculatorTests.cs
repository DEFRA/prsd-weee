﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xml.MemberRegistration;
    using Xunit;

    public class XmlChargeBandCalculatorTests
    {
        private readonly IProducerChargeBandCalculatorChooser producerChargerCalculator;
        public XmlChargeBandCalculatorTests()
        {
            producerChargerCalculator = A.Fake<IProducerChargeBandCalculatorChooser>();
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
            ProcessXmlFile request = new ProcessXmlFile(A.Dummy<Guid>(), xml, "File name");

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
            ProcessXmlFile request = new ProcessXmlFile(A.Dummy<Guid>(), xml, "File name");

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
            ProcessXmlFile request = new ProcessXmlFile(A.Dummy<Guid>(), xml, "File name");

            ProducerCharge producerCharge1 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge2 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge3 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge4 = A.Dummy<ProducerCharge>();
            ProducerCharge producerCharge5 = A.Dummy<ProducerCharge>();

            A.CallTo(() => producerChargerCalculator.GetProducerChargeBand(A<schemeType>._, A<producerType>._))
                .ReturnsNextFromSequence(producerCharge1, producerCharge2, producerCharge3, producerCharge4, producerCharge5);

            var xmlChargeBandCalculator = XmlChargeBandCalculator();

            // Act
            var producerCharges = xmlChargeBandCalculator.Calculate(request);

            // Assert
            Assert.NotNull(producerCharges);
            Assert.Equal(5, producerCharges.Count);
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
