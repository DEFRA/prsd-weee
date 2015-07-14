namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Core.Helpers.Xml;
    using Domain;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;

    public class SchemaValidator : ISchemaValidator
    {
        private const string SchemaLocation = @"v3schema.xsd";
        private readonly IXmlErrorTranslator xmlErrorTranslator;

        public SchemaValidator(IXmlErrorTranslator xmlErrorTranslator)
        {
            this.xmlErrorTranslator = xmlErrorTranslator;
        }

        public IEnumerable<MemberUploadError> Validate(ValidateXmlFile message)
        {
            var errors = new List<MemberUploadError>();

            try
            {
                // Validate against the schema
                var source = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
                var schemas = new XmlSchemaSet();
                var absoluteSchemaLocation =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), SchemaLocation);
                schemas.Add("http://www.environment-agency.gov.uk/WEEE/XMLSchema", absoluteSchemaLocation);
                source.Validate(
                    schemas,
                    (sender, args) =>
                    {
                        var asXElement = sender as XElement;
                        errors.Add(
                            asXElement != null
                                ? new MemberUploadError(ErrorLevel.Error,
                                    xmlErrorTranslator.MakeFriendlyErrorMessage(asXElement, args.Exception.Message,
                                        args.Exception.LineNumber))
                                : new MemberUploadError(ErrorLevel.Error, args.Exception.Message));
                    });
            }
            catch (XmlException ex)
            {
                errors.Add(new MemberUploadError(ErrorLevel.Error, ex.Message));
            }

            return errors;
        }
    }
}
