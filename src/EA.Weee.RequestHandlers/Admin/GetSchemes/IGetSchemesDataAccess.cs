namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using Domain.Scheme;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSchemesDataAccess
    {
        Task<List<Scheme>> GetSchemes();
    }
}
