namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Core.Shared;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Requests.DataReturns;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xml.DataReturns;
    using Xml.Deserialization;
    using Xunit;

    public class DataReturnsXmlValidatorTests
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;
        private readonly IDeserializer deserializer;

        public DataReturnsXmlValidatorTests()
        {
            schemaValidator = A.Fake<ISchemaValidator>();
            xmlConverter = A.Fake<IXmlConverter>();
            errorTranslator = A.Fake<IXmlErrorTranslator>();
            deserializer = A.Fake<IDeserializer>();
        }
               
        [Fact]
        public void SchemaValidatorHasNoErrors_DeserializationException_ShouldReturnError()
        {
            A.CallTo(() => schemaValidator.Validate(A<byte[]>._, string.Empty, string.Empty, A<string>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => deserializer.Deserialize<SchemeReturn>(A<XDocument>._))
                .Throws(new XmlDeserializationFailureException(new Exception("Test exception")));

            var result = XmlValidator().Validate(new ProcessDataReturnsXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void SchemaValidatorHasErrors_ShouldReturnErrors()
        {
            A.CallTo(() => schemaValidator.Validate(A<byte[]>._, A<string>._, A<string>._, A<string>._))
                .Returns(new List<XmlValidationError>
                {
                    new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "An error occurred")
                });

            var result = XmlValidator().Validate(new ProcessDataReturnsXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result);
        }

        private DataReturnsXmlValidator XmlValidator()
        {
            return new DataReturnsXmlValidator(schemaValidator, xmlConverter, errorTranslator, deserializer);
        }
    }
}
