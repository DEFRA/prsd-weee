namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using ReturnVersionBuilder;
    using Security;
    using Shared;
    using Xml.DataReturns;

    internal class ProcessDataReturnXmlFileHandler : IRequestHandler<ProcessDataReturnXmlFile, Guid>
    {
        private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenerateFromDataReturnXml xmlGenerator;
        private readonly Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> xmlBusinessValidatorDelegate;
        private readonly Func<Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate;

        public ProcessDataReturnXmlFileHandler(
            IProcessDataReturnXmlFileDataAccess xmlfileDataAccess,
            IWeeeAuthorization authorization,
            IGenerateFromDataReturnXml xmlGenerator,
            Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> xmlBusinessValidatorDelegate,
            Func<Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate)
        {
            this.dataAccess = xmlfileDataAccess;
            this.authorization = authorization;
            this.xmlGenerator = xmlGenerator;
            this.xmlBusinessValidatorDelegate = xmlBusinessValidatorDelegate;
            this.dataReturnVersionBuilderDelegate = dataReturnVersionBuilderDelegate;
        }

        public async Task<Guid> HandleAsync(ProcessDataReturnXmlFile message)
        {
            Scheme scheme = await dataAccess.FetchSchemeByOrganisationIdAsync(message.OrganisationId);
            authorization.EnsureSchemeAccess(scheme.Id);

            // record XML processing start time
            Stopwatch stopwatch = Stopwatch.StartNew();

            var xmlGeneratorResult = xmlGenerator.GenerateDataReturns<SchemeReturn>(message);
            DataReturnUpload dataReturnUpload;

            if (xmlGeneratorResult.SchemaErrors.Any())
            {
                dataReturnUpload = new DataReturnUpload(scheme, xmlGeneratorResult.XmlString,
                    xmlGeneratorResult.SchemaErrors.Select(e => e.ToDataReturnsUploadError()).ToList(),
                    message.FileName, null, null, null);
            }
            else
            {
                int complianceYear = int.Parse(xmlGeneratorResult.DeserialisedType.ComplianceYear);
                int quarter = Convert.ToInt32(xmlGeneratorResult.DeserialisedType.ReturnPeriod);

                var pcsReturnVersionBuilder = dataReturnVersionBuilderDelegate(scheme, new Quarter(complianceYear, (QuarterType)quarter));
                var xmlBusinessValidator = xmlBusinessValidatorDelegate(pcsReturnVersionBuilder);

                var xmlBusinessValidatorResult = await xmlBusinessValidator.Build(xmlGeneratorResult.DeserialisedType);

                var allErrors = xmlGeneratorResult.SchemaErrors.Select(e => e.ToDataReturnsUploadError())
                    .Union(xmlBusinessValidatorResult.ErrorData.Select(e => new DataReturnUploadError(e.ErrorLevel.ToDomainErrorLevel(), Domain.UploadErrorType.Business, e.Description)))
                    .ToList();

                dataReturnUpload = new DataReturnUpload(scheme, xmlGeneratorResult.XmlString, allErrors, message.FileName, null, complianceYear, quarter);

                if (!xmlBusinessValidatorResult.ErrorData.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    dataReturnUpload.SetDataReturnsVersion(xmlBusinessValidatorResult.DataReturnVersion);
                }
            }
           
            // Record XML processing end time
            stopwatch.Stop();
            dataReturnUpload.SetProcessTime(stopwatch.Elapsed);
           
            await dataAccess.AddAndSaveAsync(dataReturnUpload);

            return dataReturnUpload.Id;
        }
    }
}