namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Core.Scheme;
    using Core.Shared;
    using Errors;
    using Xml.Converter;
    using Xml.MemberRegistration;

    public class SchemaValidator : ISchemaValidator
    {
        private readonly IXmlErrorTranslator xmlErrorTranslator;
        private readonly IXmlConverter xmlConverter;
        private readonly INamespaceValidator namespaceValidator;

        public SchemaValidator(IXmlErrorTranslator xmlErrorTranslator, IXmlConverter xmlConverter, INamespaceValidator namespaceValidator)
        {
            this.xmlErrorTranslator = xmlErrorTranslator;
            this.xmlConverter = xmlConverter;
            this.namespaceValidator = namespaceValidator;
        }

        public IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, string schemaNamespace, string schemaVersion)
        {
            var errors = new List<XmlValidationError>();

            try
            {
                //check if the xml is not blank before doing any validations
                if (data != null && data.Length == 0)
                {
                    errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, XmlErrorTranslator.IncorrectlyFormattedXmlMessage));
                    return errors;
                }

                // Validate against the schema
                var source = xmlConverter.Convert(data);

                var namespaceErrors = namespaceValidator.Validate(schemaNamespace, source.Root.Name.Namespace.ToString());
                if (namespaceErrors.Any())
                {
                    return namespaceErrors;
                }

                var schemas = new XmlSchemaSet();
                using (var stream = typeof(schemeType).Assembly.GetManifestResourceStream(schemaName))
                {
                    using (var schemaReader = XmlReader.Create(stream))
                    {
                        schemas.Add(null, schemaReader);
                    }
                }

                source.Validate(
                    schemas,
                    (sender, args) =>
                    {
                        var asXElement = sender as XElement;
                        errors.Add(
                            asXElement != null
                                ? new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema,
                                    xmlErrorTranslator.MakeFriendlyErrorMessage(asXElement, args.Exception.Message,
                                        args.Exception.LineNumber, schemaVersion), args.Exception.LineNumber)
                                : new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, args.Exception.Message, args.Exception.LineNumber));
                    });
            }
            catch (XmlException ex)
            {
                string friendlyMessage = xmlErrorTranslator.MakeFriendlyErrorMessage(ex.Message, schemaVersion);

                errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, friendlyMessage));
            }
            return errors;
        }
    }
}
