namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.GenerateDomainObjects
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Domain;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Requests.DataReturns;
    using Xml.Converter;
    using Xml.MemberRegistration;
    using Xunit;

    public class GenerateFromDataReturnsXmlTest
    {
        [Fact]
        public void GenerateDataReturnsUpload_SchemaErrors_NullComplianceYear()
        {
            var message = new ProcessDataReturnsXMLFile(Guid.NewGuid(), new byte[1], "File name");

            var generateFromXml = new GenerateFromXmlBuilder().Build();

            var result = generateFromXml.GenerateDataReturnsUpload(message,
                new List<DataReturnsUploadError>
                {
                    new DataReturnsUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Some schema error")
                }, A.Dummy<Scheme>());

            Assert.Null(result.ComplianceYear);
        }

        [Fact]
        public void GenerateDataReturnsUpload_NoSchemaErrors_ComplianceYearObtained()
        {
            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.XmlConverter.Deserialize(A<XDocument>._))
                .Returns(new schemeType { complianceYear = "2015" });

            var message = new ProcessDataReturnsXMLFile(Guid.NewGuid(), new byte[1], "File name");
            var generateFromXml = builder.Build();

            var result = generateFromXml.GenerateDataReturnsUpload(message, new List<DataReturnsUploadError>(), A.Dummy<Scheme>());

            Assert.NotNull(result.ComplianceYear);
            Assert.Equal(2015, result.ComplianceYear.Value);
        }
              
        private class GenerateFromXmlBuilder
        {
            public IXmlConverter XmlConverter;
          
            public GenerateFromXmlBuilder()
            {
                XmlConverter = A.Fake<IXmlConverter>();
            }

            public GenerateFromDataReturnsXml Build()
            {
                return new GenerateFromDataReturnsXml(XmlConverter);
            }
        }
    }
}
