namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IReturnDataAccess
    {
        Task<Guid> Submit(Return aatfReturn);

        Task<Return> GetById(Guid id);

        Task<IList<Return>> GetByOrganisationId(Guid id);

        Task<IList<Return>> GetByComplianceYearAndQuarter(Return @return);
    }
}
