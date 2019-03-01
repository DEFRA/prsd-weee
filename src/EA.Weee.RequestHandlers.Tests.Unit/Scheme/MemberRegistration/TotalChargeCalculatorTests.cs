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
    using Xml.Converter;
    using Xunit;

    public class TotalChargeCalculatorTests
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IXmlConverter xmlConverter;
        private readonly IProducerChargeCalculator producerChargerCalculator;

        public TotalChargeCalculatorTests()
        {
            xmlChargeBandCalculator = A.Fake<IXMLChargeBandCalculator>();
            producerChargerCalculator = A.Fake<IProducerChargeCalculator>();
            xmlConverter = A.Fake<IXmlConverter>();
        }

        [Fact]
        public void ProcessXmlfile_ParsesXmlFileCalculateXmlChargeBand()
        {
            var request = ProcessTestXmlFile();

            var producerCharges = xmlChargeBandCalculator.Calculate(request);
            A.CallTo(() => xmlChargeBandCalculator.Calculate(request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ProcessXmlfile_ParsesXmlFileCalculateXmlChargeBandWithBusinessErrors()
        {
            var request = ProcessTestXmlFile();

            var errors = new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Business, "any description"),
            };

            A.CallTo(() => xmlChargeBandCalculator.ErrorsAndWarnings).Returns(errors);
            A.CallTo(() => xmlChargeBandCalculator.Calculate(request)).MustNotHaveHappened();
        }

        [Fact]
        public void ProcessXmlfile_ParsesXmlFile_SchemaErrors_DoesntTryToCalculateChargesOrConvertXml()
        {
            var request = ProcessTestXmlFile();

            var errors = new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, UploadErrorType.Schema, "any description"),
            };

            A.CallTo(() => xmlChargeBandCalculator.ErrorsAndWarnings).Returns(errors);

            A.CallTo(() => xmlChargeBandCalculator.Calculate(request)).MustNotHaveHappened();
            A.CallTo(() => xmlConverter.Convert(request.Data)).MustNotHaveHappened();
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