namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FetchDataReturnForSubmission;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;
    using Xml.Converter;

    internal class ProcessDataReturnXMLFileHandler : IRequestHandler<ProcessDataReturnsXMLFile, Guid>
    {
        private readonly IProcessDataReturnXMLFileDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IDataReturnXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromDataReturnXML xmlGenerator;
     
        public ProcessDataReturnXMLFileHandler(
            IProcessDataReturnXMLFileDataAccess xmlfileDataAccess,
            IWeeeAuthorization authorization,
            IDataReturnXMLValidator xmlValidator, 
            IXmlConverter xmlConverter,
            IGenerateFromDataReturnXML xmlGenerator)
        {
            this.dataAccess = xmlfileDataAccess;
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
            List<DataReturnUploadError> datareturnsUploadErrors = errors as List<DataReturnUploadError> ?? errors.ToList();
           
            DataReturnUpload dataReturnUpload = xmlGenerator.GenerateDataReturnsUpload(message, datareturnsUploadErrors, scheme);
           
            // record XML processing end time
            stopwatch.Stop();
            dataReturnUpload.SetProcessTime(stopwatch.Elapsed);
           
            if (!errors.Any())
            {
                DataReturn dataReturn = null;
                //check if data return exists for scheme, compliance year and quarter
                //get that return 
                //otherwise create a new return
                dataReturn = await dataAccess.FetchDataReturnAsync(scheme.Id, dataReturnUpload.ComplianceYear.Value, dataReturnUpload.Quarter.Value);
                if (dataReturn == null)
                {
                    dataReturn = new DataReturn(scheme, dataReturnUpload.ComplianceYear.Value, dataReturnUpload.Quarter.Value);
                }
                DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
                dataReturnUpload.SetDataReturnsVersion(dataReturnVersion);
                await dataAccess.SaveSuccessfulReturnsDataAsync(dataReturnUpload, dataReturn, dataReturnVersion);
            }
            else 
            {
                // errors
                await dataAccess.SaveDataReturnsUploadAsync(dataReturnUpload);
            }
           
            return dataReturnUpload.Id;
        }
    }
}