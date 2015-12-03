namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;
    using Xml.Converter;
    using XmlValidation;

    internal class ProcessDataReturnsXmlFileHandler : IRequestHandler<ProcessDataReturnsXMLFile, Guid>
    {
        private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IDataReturnsXmlValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromDataReturnsXml xmlGenerator;

        public ProcessDataReturnsXmlFileHandler(
            IProcessDataReturnXmlFileDataAccess dataAccess,
            IWeeeAuthorization authorization,
            IDataReturnsXmlValidator xmlValidator, 
            IXmlConverter xmlConverter,
            IGenerateFromDataReturnsXml xmlGenerator)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlGenerator = xmlGenerator;
        }

        public async Task<Guid> HandleAsync(ProcessDataReturnsXMLFile message)
        {
            Scheme scheme = await dataAccess.FetchSchemeByOrganisationIdAsync(message.OrganisationId);
            authorization.EnsureSchemeAccess(scheme.Id);

            // record XML processing start time
            Stopwatch stopwatch = Stopwatch.StartNew();

            var errors = xmlValidator.Validate(message);
            List<DataReturnsUploadError> datareturnsUploadErrors = errors as List<DataReturnsUploadError> ?? errors.ToList();

            DataReturnsUpload dataReturn = xmlGenerator.GenerateDataReturnsUpload(
                message,
                datareturnsUploadErrors,
                scheme);

            // record XML processing end time
            stopwatch.Stop();

            await dataAccess.SaveAsync(dataReturn);

            return dataReturn.Id;
        }
    }
}