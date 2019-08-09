namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IOrganisationDataAccess
    {
        Task<Organisation> GetBySchemeId(Guid schemeId);

        Task<Organisation> GetById(Guid organisationId);

        Task Delete(Guid organisationId);

        Task<bool> HasActiveUsers(Guid organisationId);

        Task<bool> HasReturns(Guid organisationId, int year);
    }
}
