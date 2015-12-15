namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;
    using Xml.Converter;

    internal class ProcessDataReturnXmlFileHandler : IRequestHandler<ProcessDataReturnXmlFile, Guid>
    {
        private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IDataReturnXmlValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromDataReturnXml xmlGenerator;
     
        public ProcessDataReturnXmlFileHandler(
            IProcessDataReturnXmlFileDataAccess xmlfileDataAccess,
            IWeeeAuthorization authorization,
            IDataReturnXmlValidator xmlValidator, 
            IXmlConverter xmlConverter,
            IGenerateFromDataReturnXml xmlGenerator)
        {
            this.dataAccess = xmlfileDataAccess;
            this.authorization = authorization;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlGenerator = xmlGenerator;
        }

        public async Task<Guid> HandleAsync(ProcessDataReturnXmlFile message)
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
                await SetDataReturnAndCurrentVersion(scheme, dataReturnUpload);
            }

            await dataAccess.AddAndSaveAsync(dataReturnUpload);

            return dataReturnUpload.Id;
        }

        private async Task SetDataReturnAndCurrentVersion(Scheme scheme, DataReturnUpload dataReturnUpload)
        {
            if (dataReturnUpload.ComplianceYear.HasValue && dataReturnUpload.Quarter.HasValue)
            {
                Quarter quarter = new Quarter(
                dataReturnUpload.ComplianceYear.Value,
                (QuarterType)dataReturnUpload.Quarter.Value);

                // Try to fetch the existing data return for the scheme and quarter, otherwise create a new data return.
                DataReturn dataReturn = await dataAccess.FetchDataReturnOrDefaultAsync(scheme, quarter);

                if (dataReturn == null)
                {
                    dataReturn = new DataReturn(scheme, quarter);
                }

                DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

                dataReturnUpload.SetDataReturnVersion(dataReturnVersion);
            }
            else
            {
                throw new ApplicationException(String.Format(
                        "Data return upload for scheme {0} does not have a compliance year or quarter",
                        scheme.Id));
            }
        }
    }
}