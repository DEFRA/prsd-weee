namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections;
    using System.Data.Entity;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class XMLChargeBandCalculatorTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly WeeeContext context;
        private readonly DbSet<ProducerChargeBand> producerChargeBandDbSet;
        private readonly DbSet<Producer> producers;

        private const string AmendmentRegistrationNumber = "WEE/HE0234YV";
     
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

            producers = helper.GetAsyncEnabledDbSet(new[] { GetPassingProducer() });

            context = A.Fake<WeeeContext>();
     
            A.CallTo(() => context.ProducerChargeBands).Returns(producerChargeBandDbSet);
            A.CallTo(() => context.Producers).Returns(producers);
        }
        
        [Fact]
        public void XMLChargeBandCalculator_ValidXml_NoErrors()
        {
            XmlChargeBandCalculator xmlChargeBandCalculator = new XmlChargeBandCalculator(context, new XmlConverter());

            RunHandler(xmlChargeBandCalculator, @"ExampleXML\v3-valid.xml");

            Assert.Empty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_XmlWithSameProducerName_AddsError()
        {
            XmlChargeBandCalculator xmlChargeBandCalculator = new XmlChargeBandCalculator(context, new XmlConverter());

            RunHandler(xmlChargeBandCalculator, @"ExampleXML\v3-same-producer-name.xml");

            Assert.NotEmpty(xmlChargeBandCalculator.ErrorsAndWarnings);
        }

        [Fact]
        public void XMLChargeBandCalculator_ValidXmlForChargeBand_GivesCorrectChargeBand()
        {
            var producerCharges = RunHandler(
                new XmlChargeBandCalculator(context, new XmlConverter()),
                @"ExampleXML\v3-valid-ChargeBand.xml");

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

        private Hashtable RunHandler(XmlChargeBandCalculator xmlChargeBandCalculator, string relativeFilePath)
        {
            var absoluteFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                relativeFilePath);

            var xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));

            return xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, xml));
        }

        private Producer GetPassingProducer()
        {
            var fakeMemberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => fakeMemberUpload.IsSubmitted).Returns(true);
            A.CallTo(() => fakeMemberUpload.ComplianceYear).Returns(2016);

            var producer = (Producer)Activator.CreateInstance(typeof(Producer), true);
            typeof(Producer).GetProperty("RegistrationNumber").SetValue(producer, AmendmentRegistrationNumber);
            typeof(Producer).GetProperty("MemberUpload").SetValue(producer, fakeMemberUpload);
            typeof(Producer).GetProperty("ChargeBandType").SetValue(producer, ChargeBandType.A.Value);
            typeof(Producer).GetProperty("ChargeThisUpdate").SetValue(producer, (decimal)5);

            return producer;
        }
    }
}
