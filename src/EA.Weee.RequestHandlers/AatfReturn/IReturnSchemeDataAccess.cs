namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReturnSchemeDataAccess
    {
        Task<Guid> Submit(ReturnScheme scheme);

        Task<List<Guid>> GetSelectedSchemesByReturnId(Guid returnId);
    }
}
