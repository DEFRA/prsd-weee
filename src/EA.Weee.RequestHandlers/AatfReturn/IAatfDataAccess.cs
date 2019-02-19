namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAatfDataAccess
    {
        Task<List<Aatf>> GetByOrganisationId(Guid id);
    }
}