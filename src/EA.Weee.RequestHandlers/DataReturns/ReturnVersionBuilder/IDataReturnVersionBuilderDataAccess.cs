namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IDataReturnVersionBuilderDataAccess
    {
        Task<DataReturn> FetchDataReturnOrDefaultAsync(Domain.Scheme.Scheme scheme, Quarter quarter);
    }
}
