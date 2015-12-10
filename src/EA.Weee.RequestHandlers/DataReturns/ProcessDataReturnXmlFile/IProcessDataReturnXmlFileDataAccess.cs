namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using EA.Weee.Domain.Scheme;

    public interface IProcessDataReturnXMLFileDataAccess
    {
        Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId);
        Task<DataReturn> FetchDataReturnAsync(Guid schemeId, int complianceYear, int quarter);
        Task SaveDataReturnsUploadAsync(DataReturnUpload dataReturnUpload);
        Task SaveSuccessfulReturnsDataAsync(DataReturnUpload dataUpload, DataReturn dataReturn, DataReturnVersion version);
    }
}
