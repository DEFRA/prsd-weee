namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSchemesDataAccess
    {
        Task<IList<Domain.Scheme.Scheme>> GetCompleteSchemes();

        Task<IList<Domain.Scheme.Scheme>> GetAllSchemes();

        Task<Domain.Scheme.Scheme> GetSchemeBasedOnId(Guid id);
    }
}
