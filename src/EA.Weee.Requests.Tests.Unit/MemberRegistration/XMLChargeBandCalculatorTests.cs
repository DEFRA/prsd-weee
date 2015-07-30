namespace EA.Weee.Requests.Tests.Unit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Helpers.Xml;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;
    using FakeItEasy;
    using Helpers;
    using PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation;
    using Xunit;

    public class XMLChargeBandCalculatorTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly WeeeContext context;
        private readonly DbSet<ProducerChargeBand> producerChargeBandDbSet;

        private readonly Guid pcsId = Guid.NewGuid();

        public XMLChargeBandCalculatorTests()
        {
            producerChargeBandDbSet = helper.GetAsyncEnabledDbSet(new[] 
            { 
                new ProducerChargeBand("A", 445), 
                new ProducerChargeBand("B", 210), 
                new ProducerChargeBand("C", 30), 
                new ProducerChargeBand("D", 30), 
                new ProducerChargeBand("E", 30) 
            });

            context = A.Fake<WeeeContext>();

            A.CallTo(() => context.ProducerChargeBands).Returns(producerChargeBandDbSet);
        }

        [Fact]
        public void XMLChargeBandCalculator_ValidXml_NoErrors()
        {
            XmlChargeBandCalculator xmlChargeBandCalculator = new XmlChargeBandCalculator(context);
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, validXml));

            Assert.Null(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_XmlWithSameProducerName_AddsError()
        {
            XmlChargeBandCalculator xmlChargeBandCalculator = new XmlChargeBandCalculator(context);
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-same-producer-name.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, validXml));

            Assert.NotEmpty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_ValidXmlForChargeBand_GivesCorrectChargeBand()
        {
            XmlChargeBandCalculator xmlChargeBandCalculator = new XmlChargeBandCalculator(context);
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid-ChargeBand.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            var producerCharges = xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, validXml));

            Assert.NotNull(producerCharges);
            Assert.Equal(producerCharges.Count, 2);
            Assert.True(producerCharges.ContainsKey("The Empire"));
            Assert.True(producerCharges.ContainsKey("Tom and Jerry"));

            var firstProducer = (ProducerCharge)producerCharges["The Empire"];
            var secondProducer = (ProducerCharge)producerCharges["Tom and Jerry"];

            Assert.Equal(firstProducer.ChargeBandType, ChargeBandType.E);
            Assert.Equal(firstProducer.ChargeAmount, 30);

            Assert.Equal(secondProducer.ChargeBandType, ChargeBandType.A);
            Assert.Equal(secondProducer.ChargeAmount, 445);
        }
    }
}
