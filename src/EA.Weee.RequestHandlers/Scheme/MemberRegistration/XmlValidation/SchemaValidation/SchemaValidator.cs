namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Domain;
    using Domain.Scheme;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using Weee.XmlValidation.Errors;
    using Xml.Converter;
    using Xml.Schemas;

    public class SchemaValidator : ISchemaValidator
    {
        private const string SchemaLocation = @"EA.Weee.Xml.Schemas.v3schema.xsd";
        private const string SchemaNamespace = @"http://www.environment-agency.gov.uk/WEEE/XMLSchema";

        private readonly IXmlErrorTranslator xmlErrorTranslator;
        private readonly IXmlConverter xmlConverter;

        public SchemaValidator(IXmlErrorTranslator xmlErrorTranslator, IXmlConverter xmlConverter)
        {
            this.xmlErrorTranslator = xmlErrorTranslator;
            this.xmlConverter = xmlConverter;
        }

        public IEnumerable<MemberUploadError> Validate(ProcessXMLFile message)
        {
            var errors = new List<MemberUploadError>();

            try
            {
                //check if the xml is not blank before doing any validations
                if (message.Data != null && message.Data.Length == 0)
                {
                    errors.Add(new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, "The file you're trying to upload is not a correctly formatted XML file. Please make sure you're uploading a valid XML file."));
                    return errors;
                }

                // Validate against the schema
                var source = xmlConverter.Convert(message.Data);
                var schemas = new XmlSchemaSet();

                using (var stream = typeof(schemeType).Assembly.GetManifestResourceStream(SchemaLocation))
                {
                    using (var schemaReader = XmlReader.Create(stream))
                    {
                        schemas.Add(null, schemaReader);
                    }
                }

                //var absoluteSchemaLocation =
                //    Path.Combine(Path.GetDirectoryName(), SchemaLocation);
                //schemas.Add(SchemaNamespace, absoluteSchemaLocation);

                string sourceNamespace = source.Root.GetDefaultNamespace().NamespaceName;
                if (sourceNamespace != SchemaNamespace)
                {
                    errors.Add(new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema,
                        string.Format("The namespace of the provided XML file ({0}) doesn't match the namespace of the WEEE schema ({1}).", sourceNamespace, SchemaNamespace)));
                    return errors;
                }

                source.Validate(
                    schemas,
                    (sender, args) =>
                    {
                        var asXElement = sender as XElement;
                        errors.Add(
                            asXElement != null
                                ? new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema,
                                    xmlErrorTranslator.MakeFriendlyErrorMessage(asXElement, args.Exception.Message,
                                        args.Exception.LineNumber), args.Exception.LineNumber)
                                : new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, args.Exception.Message, args.Exception.LineNumber));
                    });
            }
            catch (XmlException ex)
            {
                string friendlyMessage = xmlErrorTranslator.MakeFriendlyErrorMessage(ex.Message);

                errors.Add(new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, friendlyMessage));
            }
            return errors;
        }
    }
}
