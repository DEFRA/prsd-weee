namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using Core.Helpers.Xml;
    using Domain;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class XmlValidatorTests
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;

        public XmlValidatorTests()
        {
            schemaValidator = A.Fake<ISchemaValidator>();
            businessValidator = A.Fake<IBusinessValidator>();
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

            XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_AndBusinessValidatorDoes_ShouldReturnErrors()
        {
            A.CallTo(() => schemaValidator.Validate(A<ProcessXMLFile>._))
                .Returns(new List<MemberUploadError>());

            A.CallTo(() => businessValidator.Validate(A<schemeType>._, A<Guid>._))
                .Returns(new List<MemberUploadError>
                            {
                                new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Business, "An error occurred")
                            });

            var result = XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            Assert.NotEmpty(result);
        }

        private XmlValidator XmlValidator()
        {
            return new XmlValidator(schemaValidator, xmlConverter, businessValidator, errorTranslator);
        }
    }
}
