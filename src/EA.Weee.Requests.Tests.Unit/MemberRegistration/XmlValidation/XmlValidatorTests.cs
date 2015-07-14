namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using Domain;
    using FakeItEasy;
    using PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation;
    using Xunit;

    public class XmlValidatorTests
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;

        public XmlValidatorTests()
        {
            schemaValidator = A.Fake<ISchemaValidator>();
            businessValidator = A.Fake<IBusinessValidator>();
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

            A.CallTo(() => businessValidator.Validate(A<ValidateXmlFile>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void SchemaValidatorHasNoErrors_ShouldCallBusinessValidator()
        {
            A.CallTo(() => schemaValidator.Validate(A<ValidateXmlFile>._))
                .Returns(new List<MemberUploadError>());

            XmlValidator().Validate(A<ValidateXmlFile>._);

            A.CallTo(() => businessValidator.Validate(A<ValidateXmlFile>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private XmlValidator XmlValidator()
        {
            return new XmlValidator(schemaValidator, businessValidator);
        }
    }
}
