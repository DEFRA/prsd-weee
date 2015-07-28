namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.PCS;
    using Domain.Producer;
    using GenerateProducerObjects;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;
    using XmlValidation;

    internal class ProcessXMLFileHandler : IRequestHandler<ProcessXMLFile, Guid>
    {
        private readonly WeeeContext context;

        private readonly IXmlValidator xmlValidator;

        private readonly IGenerateFromXml generateFromXml;

        public ProcessXMLFileHandler(WeeeContext context, IXmlValidator xmlValidator, IGenerateFromXml generateFromXml)
        {
            this.context = context;
            this.xmlValidator = xmlValidator;
            this.generateFromXml = generateFromXml;
        }
        public async Task<Guid> HandleAsync(ProcessXMLFile message)
        {
            var errors = xmlValidator.Validate(message);

            var memberUploadErrors = errors as IList<MemberUploadError> ?? errors.ToList();

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            var upload = new MemberUpload(message.OrganisationId, message.Data, memberUploadErrors.ToList(), scheme.Id);

            //Build producers domain object if there are no errors during validation of xml file.
            if (!memberUploadErrors.Any())
            {
                var producers = await generateFromXml.Generate(message, upload);
                context.MemberUploads.Add(upload);
                context.Producers.AddRange(producers);
            }
            else
            {
                context.MemberUploads.Add(upload);
            }
            await context.SaveChangesAsync();
            return upload.Id;
        }
    }
}
