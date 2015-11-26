namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation;
    using Requests.Scheme.MemberRegistration;
    using Weee.XmlValidation.BusinessValidation;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xunit;
    using schemeType = Xml.MemberRegistration.schemeType;

    public class XmlValidatorTests
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IXmlBusinessValidator businessValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;

        public XmlValidatorTests()
        {
            schemaValidator = A.Fake<ISchemaValidator>();
            businessValidator = A.Fake<IXmlBusinessValidator>();
            xmlConverter = A.Fake<IXmlConverter>();
            errorTranslator = A.Fake<IXmlErrorTranslator>();
        }

        [Fact]
        public void SchemaValidatorHasErrors_ShouldNotCallBusinessValidator()
        {
            A.CallTo(() => schemaValidator.Validate(A<byte[]>._, string.Empty, string.Empty, A<SchemaVersion>._))
                .Returns(new List<XmlValidationError>
                {
                    new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "An error occurred")
                });

            XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_AndBusinessValidatorDoes_ShouldReturnErrors()
        {
            A.CallTo(() => schemaValidator.Validate(A<byte[]>._, string.Empty, string.Empty, A<SchemaVersion>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._))
                .Returns(new List<RuleResult>
                            {
                                RuleResult.Fail("An error occurred")
                            });

            var result = XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result);
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_DeserializationException_ShouldReturnError()
        {
            A.CallTo(() => schemaValidator.Validate(A<byte[]>._, string.Empty, string.Empty, A<SchemaVersion>._))
                .Returns(new List<XmlValidationError>());

            A.CallTo(() => xmlConverter.Deserialize(A<XDocument>._))
                .Throws(new XmlDeserializationFailureException(new Exception("Test exception")));

            var result = XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._)).MustNotHaveHappened();
        }

        private XmlValidator XmlValidator()
        {
            return new XmlValidator(schemaValidator, xmlConverter, businessValidator, errorTranslator);
        }
    }
}
