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
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;
    using XmlValidation;

    internal class ProcessXMLFileHandler : IRequestHandler<ProcessXMLFile, Guid>
    {
        private readonly WeeeContext context;

        private readonly IXmlValidator xmlValidator;

        public ProcessXMLFileHandler(WeeeContext context, IXmlValidator xmlValidator)
        {
            this.context = context;
            this.xmlValidator = xmlValidator;
        }

        public async Task<Guid> HandleAsync(ProcessXMLFile message)
        {
            var errors = xmlValidator.Validate(message);

            var memberUploadErrors = errors as IList<MemberUploadError> ?? errors.ToList();

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            var upload = new MemberUpload(message.OrganisationId, message.Data, memberUploadErrors.ToList(), scheme.Id);

             var producers = new List<Producer>();
            //Build producers domain object if there are no errors during validation of xml file.
            if (!memberUploadErrors.Any())
            {
                producers = await BuildProducerDataFromXml.SetProducerData(scheme.Id, upload, context, message.Data);
            }

            context.MemberUploads.Add(upload);

            if (producers.Count > 0)
            {
                context.Producers.AddRange(producers);
            }
            
            await context.SaveChangesAsync();
            return upload.Id;
        }
    }
}
