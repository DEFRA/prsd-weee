namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation
{
    using Core.Helpers;
    using Domain;
    using Domain.Scheme;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.XmlValidation.BusinessValidation;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xml.Deserialization;
    using Xml.MemberRegistration;

    public class XmlValidator : IXmlValidator
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IXmlBusinessValidator businessValidator;

        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;

        public XmlValidator(ISchemaValidator schemaValidator, IXmlConverter xmlConverter, IXmlBusinessValidator businessValidator, IXmlErrorTranslator errorTranslator)
        {
            this.schemaValidator = schemaValidator;
            this.businessValidator = businessValidator;
            this.errorTranslator = errorTranslator;
            this.xmlConverter = xmlConverter;
        }

        public IEnumerable<MemberUploadError> Validate(ProcessXMLFile message)
        {
            // Validate against the schema
            var errors = schemaValidator.Validate(message.Data)
                .Select(e => e.ToMemberUploadError())
                .ToList();

            if (errors.Any())
            {
                return errors;
            }

            schemeType deserializedXml;

            try
            {
                // Validate deserialized XML against business rules
                deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(message.Data));
            }
            catch (XmlDeserializationFailureException e)
            {
                // Couldn't deserialise - can't go any further, add an error and bail out here
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                var friendlyMessage = errorTranslator.MakeFriendlyErrorMessage(exceptionMessage);
                errors.Add(new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, friendlyMessage));

                return errors;
            }

            errors = businessValidator.Validate(deserializedXml, message.OrganisationId)
                .Select(err => new MemberUploadError(err.ErrorLevel.ToDomainEnumeration<ErrorLevel>(), MemberUploadErrorType.Business, err.Message))
                .ToList();

            return errors;
        }
    }
}
