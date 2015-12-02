namespace EA.Weee.RequestHandlers.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataReturns.GenerateDomainObjects;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;
    using Xml.Converter;
    using XmlValidation;

    internal class ProcessDataReturnsXMLFileHandler : IRequestHandler<ProcessDataReturnsXMLFile, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IDataReturnsXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromDataReturnsXML xmlGenerator;

        public ProcessDataReturnsXMLFileHandler(WeeeContext context, IWeeeAuthorization authorization, IDataReturnsXMLValidator xmlValidator, 
                IXmlConverter xmlConverter, IGenerateFromDataReturnsXML xmlGenerator)
        {
            this.context = context;
            this.authorization = authorization;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlGenerator = xmlGenerator;
        }

        public async Task<Guid> HandleAsync(ProcessDataReturnsXMLFile message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            // record XML processing start time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var errors = xmlValidator.Validate(message);

            List<DataReturnsUploadError> datareturnsUploadErrors = errors as List<DataReturnsUploadError> ?? errors.ToList();

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            
            var upload = xmlGenerator.GenerateDataReturnsUpload(message, datareturnsUploadErrors, scheme.Id);

            // record XML processing end time
            stopwatch.Stop();
        
            context.DataReturnsUploads.Add(upload);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Guid.NewGuid();
            }
            return upload.Id;
        }
    }
}
