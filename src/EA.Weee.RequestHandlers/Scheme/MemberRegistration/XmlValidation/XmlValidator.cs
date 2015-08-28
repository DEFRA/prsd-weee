namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Core.Helpers.Xml;
    using Domain;
    using Domain.Scheme;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;

    public class XmlValidator : IXmlValidator
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IBusinessValidator businessValidator;

        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;

        public XmlValidator(ISchemaValidator schemaValidator, IXmlConverter xmlConverter, IBusinessValidator businessValidator, IXmlErrorTranslator errorTranslator)
        {
            this.schemaValidator = schemaValidator;
            this.businessValidator = businessValidator;
            this.errorTranslator = errorTranslator;
            this.xmlConverter = xmlConverter;
        }

        public IEnumerable<MemberUploadError> Validate(ProcessXMLFile message)
        {
            // Validate against the schema
            var errors = schemaValidator.Validate(message).ToList();
            if (errors.Any())
            {
                return errors;
            }

            try
            {
                // Validate deserialized XML against business rules
                var document = xmlConverter.Convert(message);
                var deserializedXml = xmlConverter.Deserialize(document);
                errors = businessValidator.Validate(deserializedXml, message.OrganisationId).ToList();
            }
            catch (InvalidOperationException e)
            {
                var friendlyMessage = errorTranslator.MakeFriendlyErrorMessage(null, e.Message, -1);
                errors.Add(new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, friendlyMessage));
            }

            return errors;
        }
    }
}
