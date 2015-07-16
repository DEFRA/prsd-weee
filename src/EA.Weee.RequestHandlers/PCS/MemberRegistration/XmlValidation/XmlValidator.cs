namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessValidation;
    using DataValidation;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;
    using SchemaValidation;

    public class XmlValidator : IXmlValidator
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;
        private readonly IDataValidator dataValidator;
        private readonly IXmlConverter xmlConverter;

        public XmlValidator(ISchemaValidator schemaValidator, IXmlConverter xmlConverter, IBusinessValidator businessValidator, IDataValidator dataValidator)
        {
            this.schemaValidator = schemaValidator;
            this.businessValidator = businessValidator;
            this.dataValidator = dataValidator;
            this.xmlConverter = xmlConverter;
        }

        public IEnumerable<MemberUploadError> Validate(ValidateXmlFile message)
        {
            // Validate against the schema
            var errors = schemaValidator.Validate(message);        
            if (errors.Any())
            {
                return errors;
            }
            
            var deserializedXml = xmlConverter.Convert(message);

            // Validate against the deserialized XML
            errors = businessValidator.Validate(deserializedXml);            
            if (errors.Any())
            {
                return errors;
            }

            // Validate against existing data
            errors = dataValidator.Validate(deserializedXml);

            return errors;
        }
    }
}
