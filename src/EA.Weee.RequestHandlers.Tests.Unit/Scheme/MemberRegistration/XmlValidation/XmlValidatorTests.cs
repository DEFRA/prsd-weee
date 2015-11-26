namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Domain;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation;
    using Requests.Scheme.MemberRegistration;
    using Weee.XmlValidation.BusinessValidation;
    using Weee.XmlValidation.Errors;
    using Xml.Converter;
    using Xml.Deserializer;
    using Xml.Schemas;
    using Xunit;

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
            A.CallTo(() => schemaValidator.Validate(A<ProcessXMLFile>._))
                .Returns(new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, "An error occurred")
                });

            XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_AndBusinessValidatorDoes_ShouldReturnErrors()
        {
            A.CallTo(() => schemaValidator.Validate(A<ProcessXMLFile>._))
                .Returns(new List<MemberUploadError>());

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
            A.CallTo(() => schemaValidator.Validate(A<ProcessXMLFile>._))
                .Returns(new List<MemberUploadError>());

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
