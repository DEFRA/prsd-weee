namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessValidation;
    using Domain;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;
    using SchemaValidation;

    public class XmlValidator : IXmlValidator
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;

        public XmlValidator(ISchemaValidator schemaValidator, IBusinessValidator businessValidator)
        {
            this.schemaValidator = schemaValidator;
            this.businessValidator = businessValidator;
        }

        public IEnumerable<MemberUploadError> Validate(ValidateXmlFile message)
        {
            // Validate against the schema
            var errors = schemaValidator.Validate(message);

            // Validate against the deserialized XML
            if (errors.Any())
            {
                return errors;
            }

            errors = businessValidator.Validate(message);

            return errors;
        }
    }
}
