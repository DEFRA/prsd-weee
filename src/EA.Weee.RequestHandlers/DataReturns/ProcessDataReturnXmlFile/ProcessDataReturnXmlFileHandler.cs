﻿namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using ReturnVersionBuilder;
    using Security;
    using Shared;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xml.DataReturns;

    internal class ProcessDataReturnXmlFileHandler : IRequestHandler<ProcessDataReturnXmlFile, Guid>
    {
        private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenerateFromDataReturnXml xmlGenerator;
        private readonly Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> dataReturnVersionFromXmlBuilderDelegate;
        private readonly Func<Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate;

        public ProcessDataReturnXmlFileHandler(
            IProcessDataReturnXmlFileDataAccess xmlfileDataAccess,
            IWeeeAuthorization authorization,
            IGenerateFromDataReturnXml xmlGenerator,
            Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> dataReturnVersionFromXmlBuilderDelegate,
            Func<Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate)
        {
            this.dataAccess = xmlfileDataAccess;
            this.authorization = authorization;
            this.xmlGenerator = xmlGenerator;
            this.dataReturnVersionFromXmlBuilderDelegate = dataReturnVersionFromXmlBuilderDelegate;
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
                    message.FileName, null, null);
            }
            else
            {
                int complianceYear = int.Parse(xmlGeneratorResult.DeserialisedType.ComplianceYear);
                QuarterType quarter = xmlGeneratorResult.DeserialisedType.ReturnPeriod.ToDomainQuarterType();

                var pcsReturnVersionBuilder = dataReturnVersionBuilderDelegate(scheme, new Quarter(complianceYear, quarter));
                var dataReturnVersionFromXmlBuilder = dataReturnVersionFromXmlBuilderDelegate(pcsReturnVersionBuilder);

                var dataReturnVersionBuilderResult = await dataReturnVersionFromXmlBuilder.Build(xmlGeneratorResult.DeserialisedType);

                var allErrors = (dataReturnVersionBuilderResult.ErrorData
                    .Select(e => new DataReturnUploadError(e.ErrorLevel.ToDomainErrorLevel(), Domain.Error.UploadErrorType.Business, e.Description)))
                    .ToList();

                dataReturnUpload = new DataReturnUpload(scheme, xmlGeneratorResult.XmlString, allErrors, message.FileName, complianceYear, Convert.ToInt32(quarter));

                if (!dataReturnVersionBuilderResult.ErrorData.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    dataReturnUpload.SetDataReturnVersion(dataReturnVersionBuilderResult.DataReturnVersion);
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