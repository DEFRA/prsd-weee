namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Requests.DataReturns;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xml.DataReturns;
    using Xml.Deserialization;
    using Xunit;

    public class GenerateFromDataReturnsXmlTest
    {
        [Fact]
        public void GenerateDataReturns_SchemaValidatorHasNoErrors_DeserializationException_ShouldReturnError()
        {
            var builder = new GenerateFromXmlBuilder();

            A.CallTo(() => builder.SchemaValidator.Validate(A<byte[]>._, string.Empty, A<XNamespace>._, A<string>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => builder.XmlConverter.Deserialize<SchemeReturn>(A<XDocument>._))
                .Throws(new XmlDeserializationFailureException(new Exception("Test exception")));

            var result = builder.Build().GenerateDataReturns<SchemeReturn>(new ProcessDataReturnXmlFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result.SchemaErrors);
            Assert.Equal(1, result.SchemaErrors.Count);
        }

        [Fact]
        public void GenerateDataReturns_SchemaValidatorHasErrors_ShouldReturnErrors()
        {
            var builder = new GenerateFromXmlBuilder();

            A.CallTo(() => builder.SchemaValidator.Validate(A<byte[]>._, A<string>._, A<XNamespace>._, A<string>._))
                .Returns(new List<XmlValidationError>
                {
                    new XmlValidationError(Core.Shared.ErrorLevel.Error, XmlErrorType.Schema, "An error occurred")
                });

            var result = builder.Build().GenerateDataReturns<SchemeReturn>(new ProcessDataReturnXmlFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result.SchemaErrors);
        }

        [Fact]
        public void GenerateDataReturns_ReturnsExpectedXmlString()
        {
            var builder = new GenerateFromXmlBuilder();

            A.CallTo(() => builder.SchemaValidator.Validate(A<byte[]>._, string.Empty, A<XNamespace>._, A<string>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => builder.XmlConverter.Deserialize<SchemeReturn>(A<XDocument>._))
                .Returns(null);

            A.CallTo(() => builder.XmlConverter.XmlToUtf8String(A<byte[]>._))
                .Returns("Xml string");

            var result = builder.Build().GenerateDataReturns<SchemeReturn>(new ProcessDataReturnXmlFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.Equal("Xml string", result.XmlString);
        }

        [Fact]
        public void GenerateDataReturns_ReturnsExpectedDeserialisedType()
        {
            var schemeReturn = new SchemeReturn();

            var builder = new GenerateFromXmlBuilder();

            A.CallTo(() => builder.SchemaValidator.Validate(A<byte[]>._, string.Empty, A<XNamespace>._, A<string>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => builder.XmlConverter.Deserialize<SchemeReturn>(A<XDocument>._))
                .Returns(schemeReturn);

            var result = builder.Build().GenerateDataReturns<SchemeReturn>(new ProcessDataReturnXmlFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.IsType<SchemeReturn>(result.DeserialisedType);
            Assert.Equal(schemeReturn, result.DeserialisedType);
        }

        private class GenerateFromXmlBuilder
        {
            public IXmlConverter XmlConverter;
            public ISchemaValidator SchemaValidator;
            public IXmlErrorTranslator XmlErrorTranslator;

            public GenerateFromXmlBuilder()
            {
                XmlConverter = A.Fake<IXmlConverter>();
                SchemaValidator = A.Fake<ISchemaValidator>();
                XmlErrorTranslator = A.Fake<IXmlErrorTranslator>();
            }

            public GenerateFromDataReturnXml Build()
            {
                return new GenerateFromDataReturnXml(SchemaValidator, XmlConverter, XmlErrorTranslator);
            }
        }
    }
}
