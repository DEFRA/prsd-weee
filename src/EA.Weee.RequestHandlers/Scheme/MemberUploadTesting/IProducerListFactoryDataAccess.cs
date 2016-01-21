namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProducerListFactoryDataAccess
    {
        Task<List<SchemeInfo>> FetchSchemeInfo(Guid organisationId);

        Task<List<string>> GetRegistrationNumbers(Guid organisationId, int complianceYear, int numberOfRegistrationNumberToInclude);
    }
}
