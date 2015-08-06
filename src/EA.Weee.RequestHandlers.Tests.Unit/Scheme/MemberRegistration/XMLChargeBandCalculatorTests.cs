namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class XMLChargeBandCalculatorTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly WeeeContext context;
        private readonly DbSet<ProducerChargeBand> producerChargeBandDbSet;
     
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
            var xmlChargeBandCalculator = new XmlChargeBandCalculator(context, new XmlConverter());
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-valid.xml");
            var validXml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(validXmlLocation).LocalPath));

            xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, validXml));

            Assert.Empty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_XmlWithSameProducerName_AddsError()
        {
            var xmlChargeBandCalculator = new XmlChargeBandCalculator(context, new XmlConverter());
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-same-producer-name.xml");
            var invalidXml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(invalidXmlLocation).LocalPath));

            xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, invalidXml));

            Assert.NotEmpty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_ValidXmlForChargeBand_GivesCorrectChargeBand()
        {
            var xmlChargeBandCalculator = new XmlChargeBandCalculator(context, new XmlConverter());
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-valid-ChargeBand.xml");
            var validXml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(validXmlLocation).LocalPath));

            var producerCharges = xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, validXml));

            Assert.NotNull(producerCharges);
            Assert.Equal(producerCharges.Count, 5);
            Assert.True(producerCharges.ContainsKey("The Empire"));
            Assert.True(producerCharges.ContainsKey("Tom and Jerry"));
            Assert.True(producerCharges.ContainsKey("The Empire 1"));
            Assert.True(producerCharges.ContainsKey("The Empire 2"));
            Assert.True(producerCharges.ContainsKey("The Empire 3"));

            var firstProducer = (ProducerCharge)producerCharges["The Empire"];
            var secondProducer = (ProducerCharge)producerCharges["Tom and Jerry"];
            var thirdProducer = (ProducerCharge)producerCharges["The Empire 1"];
            var fourthProducer = (ProducerCharge)producerCharges["The Empire 2"];
            var fifthProducer = (ProducerCharge)producerCharges["The Empire 3"];

            Assert.Equal(firstProducer.ChargeBandType, ChargeBandType.E);
            Assert.Equal(firstProducer.ChargeAmount, 30);

            Assert.Equal(secondProducer.ChargeBandType, ChargeBandType.A);
            Assert.Equal(secondProducer.ChargeAmount, 445);

            Assert.Equal(thirdProducer.ChargeBandType, ChargeBandType.B);
            Assert.Equal(thirdProducer.ChargeAmount, 210);

            Assert.Equal(fourthProducer.ChargeBandType, ChargeBandType.D);
            Assert.Equal(fourthProducer.ChargeAmount, 30);

            Assert.Equal(fifthProducer.ChargeBandType, ChargeBandType.C);
            Assert.Equal(fifthProducer.ChargeAmount, 30);
        }
    }
}
