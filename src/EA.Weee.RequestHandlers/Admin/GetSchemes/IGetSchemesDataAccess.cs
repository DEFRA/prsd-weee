namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public interface IGetSchemesDataAccess
    {
        Task<List<Scheme>> GetSchemes();
    }
}
