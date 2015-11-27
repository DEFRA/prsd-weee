namespace EA.Weee.RequestHandlers.Admin
{
    using Domain.Scheme;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetAllApprovedSchemesDataAccess
    {
        Task<List<Scheme>> GetAllApprovedSchemes();
    }
}
