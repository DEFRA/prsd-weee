namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGetSchemesDataAccess
    {
        Task<IList<Domain.Scheme.Scheme>> GetCompleteSchemes();
    }
}
