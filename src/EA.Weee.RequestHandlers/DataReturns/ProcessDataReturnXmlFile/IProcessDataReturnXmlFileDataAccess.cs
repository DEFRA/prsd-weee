namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using Domain.DataReturns;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Threading.Tasks;

    public interface IProcessDataReturnXmlFileDataAccess
    {
        Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId);

        Task AddAndSaveAsync(DataReturnUpload dataReturnUpload);
    }
}
