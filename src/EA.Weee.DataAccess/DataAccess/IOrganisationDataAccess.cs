namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.Organisation;

    public interface IOrganisationDataAccess
    {
        Task<Organisation> GetBySchemeId(Guid schemeId);

        Task<Organisation> GetById(Guid organisationId);

        Task Delete(Guid organisationId);

        Task<bool> HasActiveUsers(Guid organisationId);

        Task<bool> HasReturns(Guid organisationId, int year);

        Task<bool> HasScheme(Guid organisationId);

        Task<bool> HasFacility(Guid organisationId, FacilityType facilityType);

        Task<List<Return>> GetReturnsByComplianceYear(Guid organisationId, int year);
    }
}
