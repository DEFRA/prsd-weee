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
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.PCS.MemberRegistration;

    internal class ValidateXmlFileHandler : IRequestHandler<ValidateXmlFile, Guid>
    {
        private const string SchemaLocation = @"v3schema.xsd";
        
        private readonly WeeeContext context;

        public ValidateXmlFileHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(ValidateXmlFile message)
        {
            var errors = new List<MemberUploadError>();

            try
            {
                var xmlDocument = XDocument.Parse(message.Data);
                var schemas = new XmlSchemaSet();
                var absoluteSchemaLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), SchemaLocation);
                schemas.Add("http://www.environment-agency.gov.uk/WEEE/XMLSchema", absoluteSchemaLocation);

                xmlDocument.Validate(
                    schemas,
                    (sender, args) =>
                    {
                        errors.Add(new MemberUploadError(ErrorLevel.Error, args.Exception.Message));
                    },
                    false);
            }
            catch (XmlException ex)
            {
                errors.Add(new MemberUploadError(ErrorLevel.Error, ex.Message));
            }
            
            MemberUpload upload = new MemberUpload(message.OrganisationId, message.Data, errors);

            context.MemberUploads.Add(upload);

            await context.SaveChangesAsync();

            return upload.Id;
        }
    }
}
