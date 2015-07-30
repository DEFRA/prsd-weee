namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Domain.PCS;
    using FakeItEasy;
    using PCS.MemberRegistration;
    using RequestHandlers;
    using RequestHandlers.PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation;
    using Xunit;

    public class XmlValidatorTests
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;
        private readonly IXmlConverter xmlConverter;

        public XmlValidatorTests()
        {
            schemaValidator = A.Fake<ISchemaValidator>();
            businessValidator = A.Fake<IBusinessValidator>();
            xmlConverter = A.Fake<IXmlConverter>();
        }

        [Fact]
        public void SchemaValidatorHasErrors_ShouldNotCallBusinessValidator()
        {
            A.CallTo(() => schemaValidator.Validate(A<ProcessXMLFile>._))
                .Returns(new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Error, "An error occurred")
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
                                new MemberUploadError(ErrorLevel.Error, "An error occurred")
                            });

            var result = XmlValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            Assert.NotEmpty(result);
        }

        private XmlValidator XmlValidator()
        {
            return new XmlValidator(schemaValidator, xmlConverter, businessValidator);
        }
    }
}
