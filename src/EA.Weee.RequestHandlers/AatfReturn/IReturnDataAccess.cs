namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReturnDataAccess
    {
        Task<Guid> Submit(Return aatfReturn);

        Task<Return> GetById(Guid id);
    }
}
