namespace EA.Weee.RequestHandlers.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.MemberRegistration;

    internal class ValidateXmlFileHandler : IRequestHandler<ValidateXmlFile, Guid>
    {
        private readonly WeeeContext context;

        public ValidateXmlFileHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(ValidateXmlFile message)
        {
            var xmlDocument = XDocument.Parse(message.Data);
            var schemas = new XmlSchemaSet();
            var schemaLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3schema.xsd");
            schemas.Add("http://www.environment-agency.gov.uk/WEEE/XMLSchema", schemaLocation);

            var errors = new List<MemberUploadError>();

            xmlDocument.Validate(
                schemas,
                (sender, args) =>
                {
                    errors.Add(new MemberUploadError(ErrorLevel.Error, args.Exception.Message));
                },
                false);

            MemberUpload upload = new MemberUpload(message.Data, errors);

            context.MemberUploads.Add(upload);

            await context.SaveChangesAsync();

            return upload.Id;
        }
    }
}
