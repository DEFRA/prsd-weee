namespace EA.Weee.RequestHandlers.Admin.Submissions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public interface IGetAllApprovedSchemesDataAccess
    {
        Task<List<Scheme>> GetAllApprovedSchemes();
    }
}
