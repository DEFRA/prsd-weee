namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
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

        [Fact]
        public void XmlChargeBandCalculator_AmendmentWithHigherBand_IncreasesChargeByDifference()
        {
            A.CallTo(() => context.ProducerChargeBands).Returns(helper.GetAsyncEnabledDbSet(GetFakeChargeBands()));

            var producerWithLowerBand = MakeSubmittedProducer(2016, AmendmentRegistrationNumber, ChargeBandType.E);

            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
            {
                producerWithLowerBand
            }));

            var producerCharges = RunHandler(
                new XmlChargeBandCalculator(context, new XmlConverter()),
                @"ExampleXML\v3-valid-AmendmentBandC.xml");

            var producerCharge = (ProducerCharge)producerCharges["The Empire"];

            // E up to C, raise by 2
            Assert.Equal(ChargeBandType.C, producerCharge.ChargeBandType);
            Assert.Equal(2, producerCharge.ChargeAmount);
        }

        [Fact]
        public void XmlChargeBandCalculator_AmendmentWithLowerBand_ZeroCharge()
        {
            A.CallTo(() => context.ProducerChargeBands).Returns(helper.GetAsyncEnabledDbSet(GetFakeChargeBands()));

            var producerWithHigherBand = MakeSubmittedProducer(2016, AmendmentRegistrationNumber, ChargeBandType.A);

            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
            {
                producerWithHigherBand
            }));

            var producerCharges = RunHandler(
                new XmlChargeBandCalculator(context, new XmlConverter()),
                @"ExampleXML\v3-valid-AmendmentBandC.xml");

            var producerCharge = (ProducerCharge)producerCharges["The Empire"];

            // A down to C, would be -2, but we don't refund, so just zero charge
            Assert.Equal(ChargeBandType.C, producerCharge.ChargeBandType);
            Assert.Equal(0, producerCharge.ChargeAmount);
        }

        [Fact]
        public void XmlChargeBandCalculator_AmendmentWithHigherBandAfterAmendmentWithLowerBand_IncreasesChargeByDifference()
        {
            A.CallTo(() => context.ProducerChargeBands).Returns(helper.GetAsyncEnabledDbSet(GetFakeChargeBands()));

            var producerWithHighishBand = MakeSubmittedProducer(2016, AmendmentRegistrationNumber, ChargeBandType.D);
            var producerWithLowerBand = MakeSubmittedProducer(2016, AmendmentRegistrationNumber, ChargeBandType.E);

            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>
            {
                producerWithHighishBand,
                producerWithLowerBand
            }));

            var producerCharges = RunHandler(
                new XmlChargeBandCalculator(context, new XmlConverter()),
                @"ExampleXML\v3-valid-AmendmentBandC.xml");

            var producerCharge = (ProducerCharge)producerCharges["The Empire"];

            // D down to E, then E up to C, highest before C was D, so D -> C, increase by 1
            Assert.Equal(ChargeBandType.C, producerCharge.ChargeBandType);
            Assert.Equal(1, producerCharge.ChargeAmount);
        }

        [Fact]
        public async void XmlChargeBandCalculator_AmendmentWithNoPreviousHistory_ThrowsArgumentException()
        {
            A.CallTo(() => context.Producers).Returns(helper.GetAsyncEnabledDbSet(new List<Producer>()));

            Assert.Throws<ArgumentException>(() => RunHandler(
                new XmlChargeBandCalculator(context, new XmlConverter()),
                @"ExampleXML\v3-valid-AmendmentBandC.xml"));
        }

        private Hashtable RunHandler(XmlChargeBandCalculator xmlChargeBandCalculator, string relativeFilePath)
        {
            var absoluteFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                relativeFilePath);

            var xml = Encoding.ASCII.GetBytes(File.ReadAllText(new Uri(absoluteFilePath).LocalPath));

            return xmlChargeBandCalculator.Calculate(new ProcessXMLFile(A<Guid>._, xml));
        }

        /// <summary>
        /// Provides a straightforward list of incrementing charges  (1, 2, 3, 4, 5) for test purposes.
        /// </summary>
        /// <returns></returns>
        private List<ProducerChargeBand> GetFakeChargeBands()
        {
            return new List<ProducerChargeBand>
            {
                new ProducerChargeBand("A", 5),
                new ProducerChargeBand("B", 4),
                new ProducerChargeBand("C", 3),
                new ProducerChargeBand("D", 2),
                new ProducerChargeBand("E", 1)
            };
        }

        private Producer GetPassingProducer()
        {
            return MakeSubmittedProducer(2016, AmendmentRegistrationNumber, ChargeBandType.A);
        }

        private Producer MakeSubmittedProducer(int complianceYear, string regNumber, ChargeBandType chargeBand)
        {
            var fakeMemberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => fakeMemberUpload.IsSubmitted).Returns(true);
            A.CallTo(() => fakeMemberUpload.ComplianceYear).Returns(complianceYear);

            // var producer = new Producer() // 18 parameters and unfakeable? ... Nnno.
            var producer = (Producer)Activator.CreateInstance(typeof(Producer), true); 
            typeof(Producer).GetProperty("RegistrationNumber").SetValue(producer, regNumber);
            typeof(Producer).GetProperty("MemberUpload").SetValue(producer, fakeMemberUpload);
            typeof(Producer).GetProperty("ChargeBandType").SetValue(producer, chargeBand.Value);

            return producer;
        }
    }
}
