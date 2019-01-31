namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IOrganisationDataAccess
    {
        Task<Organisation> GetBySchemeId(Guid schemeId);

        Task<Organisation> GetById(Guid organisationId);
    }
}
