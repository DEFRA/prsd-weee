namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using EA.Weee.Domain.Scheme;

    public interface IProcessDataReturnXmlFileDataAccess
    {
        Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId);

        Task AddAndSaveAsync(DataReturnUpload dataReturnUpload);
    }
}
