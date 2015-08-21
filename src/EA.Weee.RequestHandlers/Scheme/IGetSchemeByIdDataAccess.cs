namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGetSchemeByIdDataAccess
    {
        Task<EA.Weee.Domain.Scheme.Scheme> GetSchemeOrDefault(Guid schemeId);
    }
}
