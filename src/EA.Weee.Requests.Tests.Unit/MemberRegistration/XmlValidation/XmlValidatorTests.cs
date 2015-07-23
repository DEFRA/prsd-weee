namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation
{
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
            A.CallTo(() => schemaValidator.Validate(A<ValidateXmlFile>._))
                .Returns(new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Error, "An error occurred")
                });

            XmlValidator().Validate(A<ValidateXmlFile>._);

            A.CallTo(() => businessValidator.Validate(A<schemeType>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_AndBusinessValidatorHasNoErrors_ShouldReturnNoErrors()
        {
            A.CallTo(() => schemaValidator.Validate(A<ValidateXmlFile>._))
                .Returns(new List<MemberUploadError>());

            A.CallTo(() => businessValidator.Validate(A<schemeType>._))
                .Returns(new List<MemberUploadError>());

            var result = XmlValidator().Validate(A<ValidateXmlFile>._);

            Assert.Empty(result);
        }

        private XmlValidator XmlValidator()
        {
            return new XmlValidator(schemaValidator, xmlConverter, businessValidator);
        }
    }
}
