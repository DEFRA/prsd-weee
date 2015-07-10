namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Core.Helpers.Xml;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;

    internal class ValidateXmlFileHandler : IRequestHandler<ValidateXmlFile, Guid>
    {
        private const string SchemaLocation = @"v3schema.xsd";

        private readonly WeeeContext context;

        private readonly IXmlErrorTranslator xmlErrorTranslator;

        public ValidateXmlFileHandler(WeeeContext context, IXmlErrorTranslator xmlErrorTranslator)
        {
            this.context = context;
            this.xmlErrorTranslator = xmlErrorTranslator;
        }

        public async Task<Guid> HandleAsync(ValidateXmlFile message)
        {
            var errors = new List<MemberUploadError>();

            try
            {
                var xmlDocument = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
                var schemas = new XmlSchemaSet();
                var absoluteSchemaLocation =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), SchemaLocation);
                schemas.Add("http://www.environment-agency.gov.uk/WEEE/XMLSchema", absoluteSchemaLocation);

                xmlDocument.Validate(
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
                    },
                    false);
            }
            catch (XmlException ex)
            {
                errors.Add(new MemberUploadError(ErrorLevel.Error, ex.Message));
            }

            var upload = new MemberUpload(message.OrganisationId, message.Data, errors);

            context.MemberUploads.Add(upload);

            await context.SaveChangesAsync();

            return upload.Id;
        }
    }
}
