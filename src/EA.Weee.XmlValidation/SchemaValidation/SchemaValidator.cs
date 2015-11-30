namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
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

        public SchemaValidator(IXmlErrorTranslator xmlErrorTranslator, IXmlConverter xmlConverter)
        {
            this.xmlErrorTranslator = xmlErrorTranslator;
            this.xmlConverter = xmlConverter;
        }

        public IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, string schemaNamespace, SchemaVersion schemaVersion)
        {
            var errors = new List<XmlValidationError>();

            try
            {
                //check if the xml is not blank before doing any validations
                if (data != null && data.Length == 0)
                {
                    errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "The file you're trying to upload is not a correctly formatted XML file. Please make sure you're uploading a valid XML file."));
                    return errors;
                }

                // Validate against the schema
                var source = xmlConverter.Convert(data);
                var schemas = new XmlSchemaSet();

                using (var stream = typeof(schemeType).Assembly.GetManifestResourceStream(schemaName))
                {
                    using (var schemaReader = XmlReader.Create(stream))
                    {
                        schemas.Add(null, schemaReader);
                    }
                }

                string sourceNamespace = source.Root.GetDefaultNamespace().NamespaceName;
                if (sourceNamespace != schemaNamespace)
                {
                    errors.Add(new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema,
                        string.Format("The namespace of the provided XML file ({0}) doesn't match the namespace of the WEEE schema ({1}).", sourceNamespace, schemaNamespace)));
                    return errors;
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
